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
using HotaiPayWebView.Models;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayController : Controller
    {
        private static readonly CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
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
                if (flag)
                {
                    Session["irent_access_token"] = Request.QueryString["irent_access_token"].Trim();
                    Session["id"] = IDNO;
                }
                else
                {
                    ViewBag.Alert = errorDic[errCode];
                    return View();
                }
               
            }

            return View();
            //return RedirectToAction("CreditCardChoose", "HotaiPayCtbc");
        }

        [HttpPost]
        public ActionResult Login(Login loginVale)
        {
            if (Session["id"]==null)
            {
                ViewBag.Alert = "iRent帳號過期，請重新登入。";
                return View("Login");
            }
            HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
            if (ModelState.IsValid)
            {
                bool flag = false;
                string errCode = "";

                WebAPIInput_Signin apiInput = new WebAPIInput_Signin
                {
                    account = loginVale.Phone,
                    password = loginVale.Pwd
                };

                WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

                HashAlgorithmHelper helper = new HashAlgorithmHelper();
                apiInput.password = helper.ComputeSha256Hash(apiInput.password);
                flag = hotaiAPI.DoSignin(apiInput, ref apioutput, ref errCode);

                if (flag)
                {
                    Session["phone"] = loginVale.Phone.Trim();
                    Session["hotai_access_token"] = apioutput.access_token;
                    Session["refresh_token"] = apioutput.refresh_token;
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
                                if (!string.IsNullOrEmpty(checkVer.memberBenefitsVersion))
                                {
                                    Session["Benefitsterms"] = checkVer.memberBenefits;
                                    Session["BenefitstermsVer"] = checkVer.memberBenefitsVersion;
                                }
                                if (!string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
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
                                flag = hotaiAPI.DoGetMobilePhoneToOneID(loginVale.Phone, ref getOneID, ref errCode);
                                if (flag)
                                {
                                    Session["oneID"] = getOneID.memberSeq;

                                    errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, apioutput.access_token, apioutput.refresh_token);
                                    if (errCode == "0000")
                                    {
                                        return RedirectToRoute(new
                                        {
                                            controller = "HotaiPayCtbc",
                                            action = "NoCreditCard",
                                            irent_access_token = Session["irent_access_token"]
                                        });
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
            else
            {
                ViewBag.phone = loginVale.Phone;
                ViewBag.pwd = loginVale.Pwd;
                return View();
            }
        }
        #endregion

        #region 更新會員條款
        public ActionResult MembershipTerms1(string intoType)
        {
            if (intoType == "UpdateVer")
            {
                TempData["TermsWay"] = "UpdateVer";
                @ViewBag.Privacy = Session["Policyterms"].ToString();
            }
            else
            {
                TempData["TermsWay"] = "NewUser";
                WebAPIOutput_GetPrivacy checkVer = new WebAPIOutput_GetPrivacy();

                string errCode = "";

                var flag = hotaiAPI.DoGetPrivacy("", ref checkVer, ref errCode);
                if (flag)
                {
                    if (!string.IsNullOrEmpty(checkVer.memberBenefits) || !string.IsNullOrEmpty(checkVer.privacyPolicy))
                    {
                        Session["Benefitsterms"] = checkVer.memberBenefits;
                        Session["Policyterms"] = checkVer.privacyPolicy;
                        @ViewBag.Privacy = Session["Policyterms"].ToString();
                        return View();
                    }
                    else
                        return Redirect("RegisterStep1");
                }
                else
                {
                    return Redirect("RegisterStep1");
                }
            }
            return View();
        }

        public ActionResult AgreeTerms()
        {
            string errCode = "";
            bool flag = false;
            if (Session["BenefitstermsVer"] == null || Session["PolicytermsVer"] == null)
            {
                if (TempData["TermsWay"].ToString().Trim() == "UpdateVer")
                {

                    WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                    flag = hotaiAPI.DoGetMobilePhoneToOneID(Session["phone"].ToString().Trim(), ref getOneID, ref errCode);
                    if (flag)
                    {
                        Session["oneID"] = getOneID.memberSeq;

                        errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, Session["irent_access_token"].ToString().Trim(), Session["refresh_token"].ToString().Trim());
                        if (errCode == "0000")
                        {
                            return RedirectToRoute(new { 
                                controller = "HotaiPayCtbc", 
                                action = "NoCreditCard",
                                irent_access_token = Session["irent_access_token"]
                            });
                            //以下取得信用卡列表流程
                        }
                        else
                        {
                            return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                        }
                    }
                    else
                        return RedirectToRoute(new { controller = "HotaiPay", action = "MembershipTerms1" });
                }
                else
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep1" });
            }
            else
            {
                if (TempData["TermsWay"].ToString().Trim() == "UpdateVer")
                {
                    WebAPIInput_UpdateBenefitsAndPrivacyVersion input = new WebAPIInput_UpdateBenefitsAndPrivacyVersion
                    {
                        memberBenefitsVersion = Session["BenefitstermsVer"].ToString().Trim(),
                        privacyPolicyVersion = Session["PolicytermsVer"].ToString().Trim()
                    };
                    WebAPIOutput_BenefitsAndPrivacyVersion output = new WebAPIOutput_BenefitsAndPrivacyVersion();
                    flag = hotaiAPI.DoUpdateBenefitsAndPrivacyVersion(Session["hotai_access_token"].ToString().Trim(), input, ref output, ref errCode);

                    if (flag)
                    {
                        return RedirectToRoute(new
                        {
                            controller = "HotaiPayCtbc",
                            action = "NoCreditCard",
                            irent_access_token = Session["irent_access_token"]
                        });
                    }
                    else
                        return RedirectToRoute(new { controller = "HotaiPay", action = "MembershipTerms1" });
                }
                else
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep1" });
            }
        }

        public ActionResult MembershipTerms2()
        {
            ViewBag.Benefits = Session["Benefitsterms"].ToString();
            return View();
        }
        
        #endregion

        #region 補填會員資料頁面
        public ActionResult Supplememtary()
        {
            string errCode = "";

            WebAPIOutput_GetMemberProfile getMemberProflie = new WebAPIOutput_GetMemberProfile();
            if (Session["hotai_access_token"] != null)
            {
                var flag = hotaiAPI.DoGetMemberProfile(Session["hotai_access_token"].ToString().Trim(), ref getMemberProflie, ref errCode);
                if (flag)
                {
                    ViewBag.CustID = getMemberProflie.id;
                    ViewBag.Name = getMemberProflie.name;
                    ViewBag.Birthday = getMemberProflie.birthday;
                    ViewBag.Email = getMemberProflie.email;

                    if (string.IsNullOrEmpty(getMemberProflie.sex))
                    {
                        ViewBag.MaleCheck = false;
                        ViewBag.FemaleCheck = false;
                    }
                    else if (getMemberProflie.sex == "M")
                    {
                        ViewBag.MaleCheck = true;
                        ViewBag.FemaleCheck = false;
                    }
                    else if (getMemberProflie.sex == "F")
                    {
                        ViewBag.MaleCheck = false;
                        ViewBag.FemaleCheck = true;
                    }
                }
            }
            return View();
        }

        public ActionResult UpdateProfile(SignUpProfile signUpProfile)
        {
            bool flag = true;
            string errCode = "";

            if (ModelState.IsValid)
            {
                ViewBag.CustID = signUpProfile.CustID.Trim();
                ViewBag.Name = signUpProfile.Name.Trim();
                ViewBag.Birthday = signUpProfile.Birth.Trim();
                ViewBag.Email = signUpProfile.Email.Trim();

                if (signUpProfile.Sex.Trim() == "male")
                {
                    ViewBag.MaleCheck = true;
                    ViewBag.FemaleCheck = false;
                }
                else
                {
                    ViewBag.MaleCheck = false;
                    ViewBag.FemaleCheck = true;
                }

                if (!CheckROCID(signUpProfile.CustID.Trim()))
                {
                    ViewBag.CustIDAlert = "身分證格式錯誤";
                    return View("RegisterStep3");
                }
                else
                {
                    WebAPIInput_UpdateMemberProfile memberProfileInput = new WebAPIInput_UpdateMemberProfile
                    {
                        id = signUpProfile.CustID.Trim(),
                        name = signUpProfile.Name.Trim(),
                        sex = (signUpProfile.Sex.Trim() == "male") ? "M" : "F",
                        birthday = DateTime.ParseExact(signUpProfile.Birth.Trim(), "yyyyMMdd", CultureInfo.CurrentCulture),
                        email = signUpProfile.Email.Trim()
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
                                            return RedirectToRoute(new
                                            {
                                                controller = "HotaiPayCtbc",
                                                action = "NoCreditCard",
                                                irent_access_token = Session["irent_access_token"]
                                            });
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
            else
                return View("Supplememtary");
        }
        #endregion


        #region 註冊驗證步驟一:手機驗證
        public ActionResult RegisterStep1(string msg)
        {
            ViewBag.Phone = Session["Phone"];
            ViewBag.OtpCode = Session["OtpCode"];
            ViewBag.Alert = Session["Alert"];
            ViewData["Msg"] = msg;
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
            try
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
            catch(Exception e)
            {
                //return View("~/Views/HotaiPay/RegisterStep1.cshtml");
                return RedirectToAction("RegisterStep1", "HotaiPay", new { msg = "error" }); //參數由url帶入
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
        public ActionResult DoSignUp(ConfirmPwd confirmPwd)
        {
            if (ModelState.IsValid)
            {

                var phone = Session["Phone"].ToString();
                var otpID = Session["OtpID"].ToString();
                bool flag = false;
                WebAPIInput_Signup signUp = new WebAPIInput_Signup
                {
                    account = phone,
                    password = helper.ComputeSha256Hash(confirmPwd.Pwd),
                    confirmPassword = helper.ComputeSha256Hash(confirmPwd.PwdConfirm),
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
                        password = confirmPwd.Pwd
                    };
                    WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

                    HashAlgorithmHelper helper = new HashAlgorithmHelper();
                    apiInput.password = helper.ComputeSha256Hash(apiInput.password);
                    flag = hotaiAPI.DoSignin(apiInput, ref apioutput, ref errCode);

                    if (flag)
                    {
                        Session["hotai_access_token"] = apioutput.access_token;
                        Session["hotai_refresh_token"] = apioutput.refresh_token;
                        return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep3" });
                    }
                    else
                    {
                        return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep2" });
                    }

                }
                else
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep3" });
            }
            else
            {
                return View("RegisterStep2");
            }
        }
        #endregion

        #region 註冊驗證步驟三:會員資料填寫
        public ActionResult RegisterStep3()
        {
            return View();
        }

        public ActionResult SetSignUpProfile(SignUpProfile signUpProfile)
        {
            bool flag = false;
            string errCode = "";
            ViewBag.CustID = signUpProfile.CustID.Trim();
            ViewBag.Name = signUpProfile.Name.Trim();
            ViewBag.Birthday = signUpProfile.Birth.Trim();
            ViewBag.Email = signUpProfile.Email.Trim();

            if (signUpProfile.Sex.Trim() == "male")
            {
                ViewBag.MaleCheck = true;
                ViewBag.FemaleCheck = false;
            }
            else
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = true;
            }
            if (ModelState.IsValid)
            {

                if (!CheckROCID(signUpProfile.CustID.Trim()))
                {
                    ViewBag.CustIDAlert = "身分證格式錯誤";
                    return View("RegisterStep3");
                }

                WebAPIInput_SignupProfile memberProfileInput = new WebAPIInput_SignupProfile
                {
                    id = signUpProfile.CustID.Trim(),
                    name = signUpProfile.Name.Trim(),
                    sex = (signUpProfile.Sex.Trim() == "male") ? "M" : "F",
                    birthday = DateTime.ParseExact(signUpProfile.Birth.Trim(), "yyyyMMdd", CultureInfo.CurrentCulture),
                    email = signUpProfile.Email.Trim()
                };

                flag = hotaiAPI.DoSignupProfile(Session["hotai_access_token"].ToString().Trim(), memberProfileInput, ref errCode);

                if (flag)
                {
                    WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                    flag = hotaiAPI.DoGetMobilePhoneToOneID(Session["phone"].ToString().Trim(), ref getOneID, ref errCode);
                    if (flag)
                    {
                        Session["oneID"] = getOneID.memberSeq;
                        errCode = InsertMemberDataToDB(signUpProfile.CustID.Trim(), Session["oneID"].ToString().Trim(), Session["hotai_access_token"].ToString().Trim(), Session["hotai_refresh_token"].ToString().Trim());
                        if (errCode == "0000")
                            return RedirectToRoute(new
                            {
                                controller = "HotaiPayCtbc",
                                action = "CreditStart",
                                irent_access_token = Session["irent_access_token"]
                            });
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
            else
            {
                return View("RegisterStep3");
            }
        }

        #endregion

        #region 和泰Pay1綁定失敗
        public ActionResult BindCardFailed()
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
                cmd.Parameters.Add("@RefreshToken", SqlDbType.VarChar, 50).Value = refreshToken;
                cmd.Parameters.Add("@AccessToken", SqlDbType.VarChar, 1000).Value = accessToken;
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