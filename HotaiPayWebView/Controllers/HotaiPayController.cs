using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output.Hotai.Payment;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCommon;
using NLog;
using Reposotory.Implement;
using System.Configuration;
using Domain.TB;
using Domain.Flow.Hotai;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using Domain.TB.Hotai;
using Domain.SP.Input.Hotai;
using Microsoft.Ajax.Utilities;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayController : Controller
    {
        private static string _accessToken = "";
        private static string _refreshToken = "";

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
        private static readonly Dictionary<string, string> errorDic = commonRepository.GetErrorList("").ToLookup(x => x.ErrCode, y => y.ErrMsg).ToDictionary(x => x.Key, y => y.First());
        private static HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
        private static HashAlgorithmHelper helper = new HashAlgorithmHelper();
        private static HotaipayService HPServices = new HotaipayService();
        private static long LogID = 666;

        #region 登入頁面
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["phone"]))
            {
                ViewBag.phone = Request.QueryString["phone"].Trim();
                Session["phone"] = Request.QueryString["phone"].Trim();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["irent_access_token"]))
            {
                string IDNO = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string errCode = "";
                var flag = HPServices.GetIDNOFromToken(Request.QueryString["irent_access_token"].Trim(), LogID, ref IDNO, ref lstError, ref errCode);
                Session["irent_access_token"] = Request.QueryString["irent_access_token"].Trim();
                Session["id"] = IDNO;
            }

            return View();
            //return RedirectToAction("CreditCardChoose", "HotaiPayCtbc");
        }

        [HttpPost]
        public ActionResult Login(string phone, string pwd)
        {
            HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();

            bool flag = false;
            string errCode = "";

            WebAPIInput_Signin apiInput = new WebAPIInput_Signin
            {
                account = phone,
                password = pwd
            };
            WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

            HashAlgorithmHelper helper = new HashAlgorithmHelper();
            apiInput.password = helper.ComputeSha256Hash(apiInput.password);
            flag = hotaiAPI.DoSignin(apiInput, ref apioutput, ref errCode);

            if (flag)
            {
                Session["hotai_access_token"] = apioutput.access_token;
                Session["refresh_token"]= apioutput.refresh_token;
                if (apioutput.memberState == "1" || apioutput.memberState == "2")
                    return View("Supplememtary");
                else
                {
                    WebAPIOutput_BenefitsAndPrivacyVersion checkVer = new WebAPIOutput_BenefitsAndPrivacyVersion();
                    flag = hotaiAPI.DoCheckBenefitsAndPrivacyVersion(apioutput.access_token, ref checkVer, ref errCode);
                    
                    if (flag)
                    {
                        if (!string.IsNullOrEmpty(checkVer.memberBenefitsVersion) || !string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                        {
                            if (string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                            {
                                Session["Benefitsterms"] = checkVer.memberBenefits;
                                Session["BenefitstermsVer"] = checkVer.memberBenefitsVersion;
                            }
                            if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                            {
                                Session["Policyterms"] = checkVer.privacyPolicy;
                                Session["PolicytermsVer"] = checkVer.privacyPolicyVersion;

                            }
                            return RedirectToRoute(new
                            {
                                controller = "HotaiPay",
                                action = "MembershipTerms1",
                                intoType="UpdateVer"
                            });
                        }
                        else
                        {
                            WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                            flag = hotaiAPI.DoGetMobilePhoneToOneID(phone, ref getOneID, ref errCode);
                            if (flag)
                            {
                                Session["oneID"] = getOneID.memberSeq;

                                errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, apioutput.access_token, apioutput.refresh_token);
                                if (errCode=="0000")
                                {
                                    return Redirect($"../HotaiPayCtbc/CreditCardChoose?irent_access_token={Session["irent_access_token"].ToString().Trim()}");
                                    //以下取得信用卡列表流程
                                }
                                else
                                {
                                    return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                }

                            }
                            else
                            {
                                //RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                return RedirectToAction("BindCardFailed");
                            }
                        }
                    }
                }

            }
            else
            {
                    ViewBag.Alert = errorDic[errCode];
                    return View();
            }
            return RedirectToRoute(new { controller = "HotaiPay", action = "AlreadyMember" });
        }
        #endregion

        #region 更新會員條款
        public ActionResult MembershipTerms1(string intoType)
        {
            if (intoType== "UpdateVer")
                TempData["TermsWay"] = "UpdateVer";
            else
                TempData["TermsWay"] = "NewUser";
            return View();
        }

        public ActionResult AgreeTerms()
        {
            string errCode = "";
            WebAPIInput_UpdateBenefitsAndPrivacyVersion input = new WebAPIInput_UpdateBenefitsAndPrivacyVersion
            {
                memberBenefitsVersion = Session["BenefitstermsVer"].ToString().Trim(),
                privacyPolicyVersion = Session["PolicytermsVer"].ToString().Trim()
            };

            WebAPIOutput_BenefitsAndPrivacyVersion output = new WebAPIOutput_BenefitsAndPrivacyVersion();
            var flag = hotaiAPI.DoUpdateBenefitsAndPrivacyVersion(Session["id"].ToString().Trim(),input, ref output,ref errCode);

            if (flag)
            {
                if (TempData["TermsWay"].ToString().Trim()== "UpdateVer")
                {
                    WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                    flag = hotaiAPI.DoGetMobilePhoneToOneID(Session["phone"].ToString().Trim(), ref getOneID, ref errCode);
                    if (flag)
                    {
                        Session["oneID"] = getOneID.memberSeq;

                        errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, Session["irent_access_token"].ToString().Trim(), Session["refresh_token"].ToString().Trim());
                        if (errCode == "0000")
                        {
                            return RedirectToRoute(new { controller = "HotaiPayCtbc", action = "CreditCardChoose" });
                            //以下取得信用卡列表流程
                        }
                        else
                        {
                            return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                        }
                    }else
                        return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                }
                else 
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep1" });
            }
            else
                return RedirectToRoute(new { controller = "HotaiPay", action = "MembershipTerms2" });
        }

        public ActionResult MembershipTerms2()
        {
            return View();
        }
        
        #endregion

        #region 補填會員資料頁面
        public ActionResult Supplememtary()
        {
            string errCode = "";

            WebAPIOutput_GetMemberProfile getMemberProflie = new WebAPIOutput_GetMemberProfile();
            var flag = hotaiAPI.DoGetMemberProfile(Session["hotai_access_token"].ToString().Trim(), ref getMemberProflie, ref errCode);
            if (flag)
            {
                ViewBag.CustID = getMemberProflie.id;
                ViewBag.Name = getMemberProflie.name;
                ViewBag.Birthday = getMemberProflie.birthday;
                ViewBag.Email = getMemberProflie.email;

                if (string.IsNullOrEmpty(getMemberProflie.sex)){
                    ViewBag.MaleCheck = false;
                    ViewBag.FemaleCheck =false;
                }else if (getMemberProflie.sex == "M"){
                    ViewBag.MaleCheck = true;
                    ViewBag.FemaleCheck = false;
                }
                else if (getMemberProflie.sex == "F"){
                    ViewBag.MaleCheck = false;
                    ViewBag.FemaleCheck = true;
                }
            }
            return View();
        }

        public ActionResult UpdateProfile(FormCollection form)
        {
            bool flag = true;
            string errCode = "";

            ViewBag.CustID = form["custID"].Trim();
            ViewBag.Name = form["name"].Trim();
            ViewBag.Birthday = form["birth"].Trim();
            ViewBag.Email = form["email"].Trim();

            if (form["sex"].Trim() == "male")
            {
                ViewBag.MaleCheck = true;
                ViewBag.FemaleCheck = false;
            }
            else
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = true;
            }

            if (!CheckROCID(form["custID"].Trim()))
            {
                ViewBag.CustIDAlert = "身分證格式錯誤";
                return View("RegisterStep3");
            }
            else
            {
                WebAPIInput_UpdateMemberProfile memberProfileInput = new WebAPIInput_UpdateMemberProfile
                {
                    id = form["custID"].Trim(),
                    name = form["name"].Trim(),
                    sex = (form["sex"].Trim() == "male") ? "M" : "F",
                    birthday = DateTime.ParseExact(form["birth"].Trim(), "yyyyMMdd", CultureInfo.CurrentCulture),
                    email = form["email"].Trim()
                };

                flag = hotaiAPI.DoUpdateMemberProfile(Session["hotai_access_token"].ToString().Trim(), memberProfileInput, ref errCode);

                if (flag)
                {
                    WebAPIOutput_IsMissingMemberProfile isMissingProfile = new WebAPIOutput_IsMissingMemberProfile();
                    flag = hotaiAPI.DoIsMissingMemberProfile(Session["hotai_access_token"].ToString().Trim(), ref isMissingProfile, ref errCode);
                    if (flag && isMissingProfile.memberState == "3")
                    {
                        WebAPIOutput_BenefitsAndPrivacyVersion checkVer = new WebAPIOutput_BenefitsAndPrivacyVersion();
                        flag = hotaiAPI.DoCheckBenefitsAndPrivacyVersion(Session["hotai_access_token"].ToString().Trim(), ref checkVer, ref errCode);

                        if (flag)
                        {
                            if (!string.IsNullOrEmpty(checkVer.memberBenefitsVersion) || !string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                            {
                                if (string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                                {
                                    Session["Benefitsterms"] = checkVer.memberBenefits;
                                    Session["BenefitstermsVer"] = checkVer.memberBenefitsVersion;
                                }
                                if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                {
                                    Session["Policyterms"] = checkVer.privacyPolicy;
                                    Session["PolicytermsVer"] = checkVer.privacyPolicyVersion;
                                }
                                return RedirectToRoute(new
                                {
                                    controller = "HotaiPay",
                                    action = "MembershipTerms1",
                                    intoType = "UpdateVer"
                                });
                            }
                            else
                            {
                                WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                                flag = hotaiAPI.DoGetMobilePhoneToOneID(Session["phone"].ToString().Trim(), ref getOneID, ref errCode);
                                if (flag)
                                {
                                    Session["oneID"] = getOneID.memberSeq;

                                    errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, Session["hotai_access_token"].ToString().Trim(), Session["refresh_token"].ToString().Trim());
                                    if (errCode == "0000")
                                    {
                                        return Redirect($"../HotaiPayCtbc/CreditCardChoose?irent_access_token={Session["irent_access_token"].ToString().Trim()}");
                                        //以下取得信用卡列表流程
                                    }
                                    else
                                    {
                                        return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                    }

                                }
                                else
                                {
                                    return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                }
                            }
                        }
                        else
                        {
                            return View("Supplememtary");
                        }
                    }
                    else
                    {
                        if (isMissingProfile.missingId)
                        {
                            ViewBag.CustID = "缺少身份證字號";
                        }
                        if (isMissingProfile.missingName)
                        {
                            ViewBag.Name = "缺少姓名";
                        }
                        if (isMissingProfile.missingBirthday)
                        {
                            ViewBag.Birthday = "缺少生日";
                        }
                        if (isMissingProfile.missingEmail)
                        {
                            ViewBag.Email = "缺少電子郵件";
                        }
                        if (isMissingProfile.missingSex)
                        {
                            ViewBag.MaleCheck = false;
                            ViewBag.FemaleCheck = false;
                        }
                        return View();
                    }
                }
                else
                {
                    return View("Supplememtary");
                }
            }
        }
        #endregion


        #region 註冊驗證步驟一:手機驗證
        public ActionResult RegisterStep1()
        {
            ViewBag.Phone = Session["Phone"];
            ViewBag.OtpCode = Session["OtpCode"];
            ViewBag.Alert = Session["Alert"];
            return View();
        }

        [HttpPost]
        public ActionResult GetOtpCode(string phone)
        {
            Session["Phone"] = phone;
            string errCode = "";
            bool flag = false;
            WebAPIInput_CheckSignup checkSignUp = new WebAPIInput_CheckSignup
            {
                account = phone
            };
            WebAPIOutput_CheckSignup checkSignUpOutput = new WebAPIOutput_CheckSignup();
            flag = hotaiAPI.DoCheckSignup(checkSignUp, ref checkSignUpOutput, ref errCode);

            if (checkSignUpOutput.isSignup)
            {
                Session["Alert"] = "此帳號已被註冊";
                return Redirect("RegisterStep1");
            }
            else
            {
                WebAPIInput_SendSmsOtp getSMSOTP = new WebAPIInput_SendSmsOtp
                {
                    mobilePhone = phone,
                    otpId = "",
                    useType = 1
                };
                WebAPIOutput_SendSmsOtp getSMSOTPoutput = new WebAPIOutput_SendSmsOtp();

                flag = hotaiAPI.DoSendSmsOtp(getSMSOTP, ref getSMSOTPoutput, ref errCode);
                if (flag)
                {
                    return Redirect("RegisterStep1");
                }
                else
                {
                    Session["Alert"] = errorDic[errCode];

                    return Redirect("RegisterStep1");
                }
            }
        }

        [HttpPost]
        public ActionResult CheckOtpCode(string otpCode)
        {


            bool flag = false;
            WebAPIInput_SmsOtpValidation checkSMSOpt = new WebAPIInput_SmsOtpValidation
            {
                mobilePhone = Session["Phone"].ToString(),
                otpCode = otpCode,
                useType = 1
            };
            WebAPIOutput_OtpValidation checkSMSOptOutput = new WebAPIOutput_OtpValidation();
            string errCode = "";
            flag = hotaiAPI.DoSmsOtpValidation(checkSMSOpt, ref checkSMSOptOutput, ref errCode);

            if (flag)
            {
                Session["OtpCode"] = otpCode;
                Session["OtpID"] = checkSMSOptOutput.otpId;
                return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep2" });
            }
            else
            {
                return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep2" });
            }

        }
        #endregion

        #region 註冊驗證步驟二:密碼設定
        [Route("~/Home/RegisterStep2")]
        public ActionResult RegisterStep2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoSignUp(string pwd, string comfirmPwd)
        {
            var phone = Session["Phone"].ToString();
            var otpID = Session["OtpID"].ToString();
            bool flag = false;
            WebAPIInput_Signup signUp = new WebAPIInput_Signup
            {
                account = phone,
                password = helper.ComputeSha256Hash(pwd),
                confirmPassword = helper.ComputeSha256Hash(comfirmPwd),
                otpId = otpID
            };

            WebAPIOutput_Token getToken = new WebAPIOutput_Token();
            string errCode = "";

            flag = hotaiAPI.DoSignup(signUp, ref getToken, ref errCode);

            if (flag)
            {
                WebAPIInput_Signin apiInput = new WebAPIInput_Signin
                {
                    account = phone,
                    password = pwd
                };
                WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

                HashAlgorithmHelper helper = new HashAlgorithmHelper();
                apiInput.password = helper.ComputeSha256Hash(apiInput.password);
                flag = hotaiAPI.DoSignin(apiInput, ref apioutput, ref errCode);

                if (flag)
                {
                    Session["hotai_access_token"] = apioutput.access_token;
                    Session["hotai_refresh_token"] = apioutput.refresh_token;
                }
                else
                {
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep2" });
                }
                
            }
            return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep3" });
        }
        #endregion

        #region 註冊驗證步驟三:會員資料填寫
        public ActionResult RegisterStep3()
        {
            return View();
        }

        public ActionResult SetSignUpProfile(FormCollection form)
        {
            bool flag = true;
            string errCode = "";

            ViewBag.CustID = form["custID"].Trim();
            ViewBag.Name = form["name"].Trim();
            ViewBag.Birthday = form["birth"].Trim();
            ViewBag.Email = form["email"].Trim();

            if (form["sex"].Trim() == "male")
            {
                ViewBag.MaleCheck = true;
                ViewBag.FemaleCheck = false;
            }
            else
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = true;
            }

            if (!CheckROCID(form["custID"].Trim()))
            {
                ViewBag.CustIDAlert = "身分證格式錯誤";
                return View("RegisterStep3");
            }

            WebAPIInput_SignupProfile memberProfileInput = new WebAPIInput_SignupProfile
            {
                id = form["custID"].Trim(),
                name = form["name"].Trim(),
                sex = (form["sex"].Trim() == "male") ? "M" : "F",
                birthday = DateTime.ParseExact(form["birth"].Trim(), "yyyyMMdd", CultureInfo.CurrentCulture),
                email = form["email"].Trim()
            };

            flag = hotaiAPI.DoSignupProfile(Session["hotai_access_token"].ToString().Trim(), memberProfileInput, ref errCode);

            if (flag)
            {
                WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                flag = hotaiAPI.DoGetMobilePhoneToOneID(Session["phone"].ToString().Trim(), ref getOneID, ref errCode);
                if (flag)
                {
                    Session["oneID"] = getOneID.memberSeq;
                    errCode = InsertMemberDataToDB(Session["id"].ToString().Trim(), Session["oneID"].ToString().Trim(), Session["hotai_access_token"].ToString().Trim(), Session["hotai_refresh_token"].ToString().Trim());
                    if (errCode == "0000")
                    {
                        WebAPIOutput_BenefitsAndPrivacyVersion checkVer = new WebAPIOutput_BenefitsAndPrivacyVersion();
                        flag = hotaiAPI.DoCheckBenefitsAndPrivacyVersion(Session["hotai_access_token"].ToString().Trim(), ref checkVer, ref errCode);
                        
                        if (flag)
                        {
                            if (!string.IsNullOrEmpty(checkVer.memberBenefits) || !string.IsNullOrEmpty(checkVer.privacyPolicy))
                            {
                                if (string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                                {
                                    Session["Benefitsterms"] = checkVer.memberBenefits;
                                    Session["BenefitstermsVer"] = checkVer.memberBenefitsVersion;
                                }
                                if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                {
                                    Session["Policyterms"] = checkVer.privacyPolicy;
                                    Session["PolicytermsVer"] = checkVer.privacyPolicyVersion;

                                }
                                return RedirectToRoute(new
                                {
                                    controller = "HotaiPay",
                                    action = "MembershipTerms1",
                                    intoType = "UpdateVer"
                                });
                            }
                            else
                                return Redirect("CreditStart");
                        }
                        else
                        {
                            return Redirect("RegisterStep3");
                        }
                        
                    }
                    else
                        return Redirect("RegisterSuccessBindFail");
                }
                else 
                    return View("RegisterStep3");
            }
            else
            {
                return View("RegisterFail");
            }
        }

        #endregion

        #region 綁卡失敗
        public ActionResult BindCardFailed()
        {
            return View();
        }
        #endregion

        #region 選擇新的信用卡
        public ActionResult BindNewCard()
        {
            return View();
        }
        #endregion

        #region 個人資料填寫
        public ActionResult PersonalInformation()
        {
            return View();
        }
        #endregion

        #region 開始綁定信用卡
        public ActionResult CreditStart()
        {
            ViewBag.HotaiAccessToken = Session["hotai_access_token"].ToString().Trim();
            return View();
        }
        #endregion

        #region 無信用卡列表頁面 
        public ActionResult NoCreditCard(string irent_access_token)
        {
            HotaipayService HPServices = new HotaipayService();
            bool flag = false;
            string errCode = "";
            string PRGName = "NoCreditCard";
            List<ErrorInfo> errList = new List<ErrorInfo>();
            var IDNO = "";
            flag = HPServices.GetIDNOFromToken(_accessToken, LogID, ref IDNO,ref errList, ref errCode);

            if(System.Web.HttpContext.Current.Session["irent_access_token"] == null )
                System.Web.HttpContext.Current.Session["irent_access_token"] = Request.QueryString["irent_access_token"] ;

            if (!string.IsNullOrEmpty(Request.QueryString["irent_access_token"]))
            {
                flag = HPServices.GetIDNOFromToken(Request.QueryString["irent_access_token"].Trim(), LogID, ref IDNO, ref errList, ref errCode);
                System.Web.HttpContext.Current.Session["IDNO"] = IDNO;
            }
            //取得和泰Token
            var hotaiToken = new HotaiToken();
            flag = HPServices.DoQueryToken(IDNO, PRGName, ref hotaiToken, ref errCode);
            if (!flag)
            {
                logger.Error("HotaiPay.NoCreditCard.DoQueryToken fail");
                return Redirect("/HotaiPay/Login?irent_access_token=" + Request.QueryString["irent_access_token"]);
            }

            //取得卡片清單
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();

            //設定查詢的IDNO
            //input.IDNO = "C221120413";//測試用資料 上線需更改
            input.IDNO = IDNO;//測試用資料 上線需更改
            flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
           
            if (flag)
            {
                if (output.CreditCards.Count > 0)
                {
                    List<HotaiCardInfo> L_Output = output.CreditCards;
                    if(L_Output.Count > 0)
                        return View("CreditCardChoose", L_Output);
                }
            }
            else {
                logger.Error("HotaiPay.NoCreditCard.DoQueryCardList 查詢卡清單失敗 ERRCODE:" + errCode);
            }
            return View();
        }
        #endregion

        #region 選擇綁定卡片
        [HttpPost]
        public ActionResult CreditcardChoose(FormCollection form)
        {
            string IDNO = System.Web.HttpContext.Current.Session["IDNO"].ToString();
            string irent_access_token = System.Web.HttpContext.Current.Session["irent_access_token"].ToString();
            Boolean flag = true;
            string errCode = "";
            HotaipayService HPServices = new HotaipayService();
            string thatCardValue = form["CreditCardList"].Trim();
            if (thatCardValue != "")
            {
                string[] input = thatCardValue.Split('|');
                string MemberOneID = input[0];
                string CardType = input[1];
                string BankDesc = input[2];
                string CardNumber = input[3];
                string CardToken = input[4];

                var sp_input = new SPInput_SetDefaultCard();
                    sp_input.IDNO       = IDNO;
                    sp_input.OneID      = MemberOneID;
                    sp_input.CardToken  = CardToken;
                    sp_input.CardNo     = CardNumber;
                    sp_input.CardType   = CardType;
                    sp_input.BankDesc   = BankDesc;
                    sp_input.PRGName    = "CreditcardChoose";

                flag = HPServices.sp_SetDefaultCard(sp_input,ref errCode);
                if(!flag)
                    logger.Error("HotaiPay.CreditcardChoose.sp_SetDefaultCard 設定預設卡失敗 ERRCODE:"+ errCode);
            }
            if (flag)
                return Redirect("/HotaiPay/RegisterSuccess");
            else
                return Redirect("/HotaiPay/BindCardFailed");
        }
        #endregion

        #region 綁定成功
        public ActionResult SuccessBind()
        {
            return View();
        }
        #endregion

        #region 已是和泰會員
        public ActionResult AlreadyMember()
        {
            string accessToken = "";
            accessToken = Request.QueryString["irent_access_token"];       
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewData["Token"] = accessToken.Trim();
                return View();
            }
        }
        #endregion

        #region 解綁
        [HttpPost]
        public JsonResult Unbind(string token)
        {
            bool flag = false;
            string errCode = "";
            string IDNO = "";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (!string.IsNullOrWhiteSpace(token))
            {
                flag = HPServices.GetIDNOFromToken(token, 0, ref IDNO, ref lstError, ref errCode);
            }
            else
            {
                flag = false; //導登入頁
            }

            if (flag)
            {
                SPInput_MemberUnBind sp_unBindinput = new SPInput_MemberUnBind() { IDNO = IDNO, PRGName = "Unbind" };
                flag = HPServices.sp_MemberUnBind(sp_unBindinput, ref errCode);                
            }

            if (flag)
            {
                return Json(new { redirectUrl = Url.Action("UnbindSuccess", "HotaiPay") });
            }
            else
            {
                return Json(flag);
            }          
        }
        #endregion

        #region 成功解綁頁面
        public ActionResult UnbindSuccess()
        {
            return View();
        }
        #endregion

        #region 立即註冊會員條款同意頁面
        public ActionResult RegisterMembershipTerm()
        {
            bool flag = false;
            WebAPIOutput_GetPrivacy getPrivacy = new WebAPIOutput_GetPrivacy();
            string errCode = "";

            flag = hotaiAPI.DoGetPrivacy("", ref getPrivacy, ref errCode);

            return View();
        }

        #endregion


        #region 註冊成功
        public ActionResult RegisterSuccess()
        {
            return View();
        }
        #endregion

        #region 註冊失敗
        public ActionResult RegisterFail()
        {
            return View();
        }
        #endregion

        #region 註冊成功綁卡失敗
        public ActionResult RegisterSuccessBindFail()
        {
            return View();
        }
        #endregion

        public string InsertMemberDataToDB(string id,string oneID,string accessToken,string refreshToken)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IRentT"].ConnectionString;
            var logID = 999;
            using (SqlConnection conn=new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_InsHotaiMember_I01", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IDNO", SqlDbType.VarChar, 10).Value = id;
                cmd.Parameters.Add("@OneID", SqlDbType.VarChar, 50).Value = oneID;
                cmd.Parameters.Add("@RefreshToken", SqlDbType.VarChar, 50).Value = accessToken;
                cmd.Parameters.Add("@AccessToken", SqlDbType.VarChar, 1000).Value = refreshToken;
                cmd.Parameters.Add("@LogID", SqlDbType.BigInt).Value = logID;
                cmd.Parameters.Add("@ErrorCode", SqlDbType.VarChar, 8);
                cmd.Parameters["@ErrorCode"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorMsg", SqlDbType.VarChar, 100);
                cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SQLExceptionCode", SqlDbType.VarChar, 10);
                cmd.Parameters["@SQLExceptionCode"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SQLExceptionMsg", SqlDbType.VarChar, 1000);
                cmd.Parameters["@SQLExceptionMsg"].Direction = ParameterDirection.Output;


                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return cmd.Parameters["@ErrorCode"].Value.ToString();
                }
                catch (Exception)
                {

                    throw;
                }
            }
                
        }

        public bool CheckROCID(string idNo)
        {
            if (idNo == null)
            {
                return false;
            }
            idNo = idNo.ToUpper();
            Regex regex = new Regex(@"^([A-Z])([1-2]\d{8})$");
            Match match = regex.Match(idNo);
            if (!match.Success)
            {
                return false;
            }

            ///建立字母對應表(A~Z)
            ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35 
            string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
            string transferIdNo = $"{(alphabet.IndexOf(match.Groups[1].Value) + 10)}" +
                                  $"{match.Groups[2].Value}";
            int[] idNoArray = transferIdNo.ToCharArray()
                                          .Select(c => Convert.ToInt32(c.ToString()))
                                          .ToArray();
            int sum = idNoArray[0];
            int[] weight = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            for (int i = 0; i < weight.Length; i++)
            {
                sum += weight[i] * idNoArray[i + 1];
            }
            return (sum % 10 == 0);
        }
    }
}