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
using Newtonsoft.Json;
using Domain.SP.Output.Hotai;

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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region 登入頁面
        public ActionResult Login()
        {
            Dictionary<string, string> decryptDic = new Dictionary<string, string>();

            logger.Debug($"p={Request.QueryString["p"]}");

            if (!Request.QueryString["p"].IsNullOrWhiteSpace())
            {
                logger.Error($"接到p參數: p={Request.QueryString["p"]}");
                decryptDic = HPServices.QueryStringDecryption(Request.QueryString["p"].Trim());
                Session["p"] = Request.QueryString["p"].Trim();
            }

            if (decryptDic.Count == 0)
            {

                logger.Error($"p參數解密失敗，p={Request.QueryString["p"]}");

                ViewBag.PwdAlert = "iRent帳號過期，請重新登入";
                return View();
            }

            if (decryptDic.ContainsKey("phone"))
            {
                ViewBag.phone = decryptDic["phone"].Trim();
                Session["phone"] = decryptDic["phone"].Trim();
            }

            if (decryptDic.ContainsKey("name"))
                Session["name"] = decryptDic["name"].Trim();
            if (decryptDic.ContainsKey("birth"))
                Session["birth"] = decryptDic["birth"].Trim();
            if (decryptDic.ContainsKey("email"))
                Session["email"] = decryptDic["email"].Trim();

            if (decryptDic.ContainsKey("irent_access_token"))
            {
                string IDNO = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string errCode = "";

                var flag = HPServices.GetIDNOFromToken(decryptDic["irent_access_token"].Trim(), LogID, ref IDNO, ref lstError, ref errCode);

                if (flag)
                {
                    ViewBag.PwdAlert = "";
                    ViewBag.PhoneAlert = "";

                    logger.Info($"irent_access_token解密成功: id={IDNO},irent_access_token={decryptDic["irent_access_token"].Trim()}");

                    Session["irent_access_token"] = decryptDic["irent_access_token"].Trim();
                    Session["id"] = IDNO;
                    return View();
                }
                else
                {
                    ViewBag.phone = decryptDic["phone"].Trim();

                    if (errorDic[errCode].Contains("帳號"))
                        ViewBag.PhoneAlert = errorDic[errCode];
                    else if (errorDic[errCode].Contains("密碼"))
                        ViewBag.PwdAlert = errorDic[errCode];
                    else
                        ViewBag.PwdAlert = errorDic[errCode];

                    logger.Error($"irent_access_token解密失敗: ErrorMessage:{errorDic[errCode]}");

                    return View();
                }
            }
            else
            {
                ViewBag.PwdAlert = "iRent帳號過期，請重新登入";
                return View();
            }
            //return RedirectToAction("CreditCardChoose", "HotaiPayCtbc");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(Login loginValue)
        {
            //logger.Info($"tanglogin : {JsonConvert.SerializeObject(loginVale)}");
            if (Session["id"] == null)
            {
                logger.Error("iRent帳號過期,Session[id] == null");
                ViewBag.PwdAlert = "iRent帳號過期，請重新登入。";
                return View("Login");
            }
            HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
            if (ModelState.IsValid)
            {
                bool flag = false;
                string errCode = "";

                WebAPIInput_Signin apiInput = new WebAPIInput_Signin
                {
                    account = loginValue.Phone,
                    password = loginValue.Pwd
                };

                WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

                HashAlgorithmHelper helper = new HashAlgorithmHelper();
                apiInput.password = helper.ComputeSha256Hash(apiInput.password);
                flag = hotaiAPI.DoSignin(apiInput, ref apioutput, ref errCode);

                if (flag)
                {
                    logger.Info($"登入成功: id={Session["id"]}, hotai_access_token={apioutput.access_token},refresh_token={apioutput.refresh_token},memberState={apioutput.memberState}");

                    Session["phone"] = loginValue.Phone.Trim();
                    Session["hotai_access_token"] = apioutput.access_token;
                    Session["refresh_token"] = apioutput.refresh_token;
                    if (apioutput.memberState == "1" || apioutput.memberState == "2")
                        return RedirectToRoute(new { controller = "HotaiPay", action = "Supplememtary" });
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
                                    logger.Info($"id={Session["id"]},有更新的會員條款版本號");

                                    Session["Benefitsterms"] = checkVer.memberBenefits;
                                    Session["BenefitstermsVer"] = checkVer.memberBenefitsVersion;
                                }
                                if (!string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                {
                                    logger.Info($"id={Session["id"]},有更新的隱私條款版本號");

                                    Session["Policyterms"] = checkVer.privacyPolicy;
                                    Session["PolicytermsVer"] = checkVer.privacyPolicyVersion;
                                }
                                return RedirectToRoute(new
                                {
                                    controller = "HotaiPay",
                                    action = "MembershipTerms2",
                                    intoType = "UpdateVer"
                                });
                            }
                            else
                            {
                                WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                                flag = hotaiAPI.DoGetMobilePhoneToOneID(loginValue.Phone, ref getOneID, ref errCode);
                                //flag = true;//唐寫死，等和泰開通安康防火牆再弄
                                if (flag)
                                {
                                    logger.Info($"取得OneID成功: id={Session["id"]},OneID={getOneID.memberSeq}");

                                    //Session["oneID"] = "";//getOneID.memberSeq;//唐寫死，等和泰開通安康防火牆再弄
                                    Session["oneID"] = getOneID.memberSeq;

                                    errCode = InsertMemberDataToDB(Session["id"].ToString(), getOneID.memberSeq, apioutput.access_token, apioutput.refresh_token);

                                    if (errCode == "0000")
                                    {
                                        logger.Info($"寫入和泰會員綁定資料表成功: id={Session["id"]},errCode={errCode}");

                                        TempData["irent_access_token"] = Session["irent_access_token"];
                                        return RedirectToRoute(new { controller = "HotaiPayCtbc", action = "NoCreditCard" });
                                        //以下取得信用卡列表流程
                                    }
                                    else if (errCode == "9527")
                                    {
                                        logger.Info($"此OneID已有資料在資料表中: id={Session["id"]},errCode={errCode}");

                                        ViewBag.phone = loginValue.Phone.Trim();
                                        ViewBag.PwdAlert = "此門號已被其他iRent帳號綁定。";
                                        return View();
                                    }
                                    else
                                    {
                                        logger.Error($"寫入和泰綁定資料表失敗: id={Session["id"]},errCode={errCode}");
                                        //return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                        return RedirectToAction("BindCardFailed", new { });
                                    }
                                }
                                else
                                {
                                    logger.Error($"取得OneID失敗: id={Session["id"]},ErrorMessage:{errorDic[errCode]}");
                                    //RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
                                    return RedirectToAction("BindCardFailed", new { });
                                }
                            }
                        }
                    }

                }
                else
                {
                    logger.Error($"登入失敗: id={Session["id"]},ErrorMessage:{errorDic[errCode]}");

                    ViewBag.phone = loginValue.Phone.Trim();

                    if (errorDic[errCode].Contains("帳號"))
                        ViewBag.PhoneAlert = errorDic[errCode];
                    else if (errorDic[errCode].Contains("密碼"))
                        ViewBag.PwdAlert = errorDic[errCode];
                    else
                        ViewBag.PwdAlert = errorDic[errCode];
                    return View();
                }
                return RedirectToRoute(new { controller = "HotaiPay", action = "AlreadyMember" });
            }
            else
            {
                ViewBag.phone = loginValue.Phone;
                ViewBag.pwd = loginValue.Pwd;
                return View();
            }
        }
        #endregion

        #region 更新會員條款
        public ActionResult MembershipTerms2(string intoType)
        {
            if (intoType == "UpdateVer")
            {
                WebAPIOutput_GetPrivacy checkVer = new WebAPIOutput_GetPrivacy();
                TempData["TermsWay"] = "UpdateVer";
                string errCode = "";
                var flag = hotaiAPI.DoGetPrivacy("", ref checkVer, ref errCode);

                if (flag)
                {
                    if (!string.IsNullOrEmpty(checkVer.memberBenefits) || !string.IsNullOrEmpty(checkVer.privacyPolicy))
                    {
                        Session["Benefitsterms"] = checkVer.memberBenefits;
                        Session["Policyterms"] = checkVer.privacyPolicy;
                        ViewBag.Benefits = Session["Benefitsterms"].ToString();
                        return View();
                    }
                    else
                        return Redirect("RegisterStep1");
                }

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
                        ViewBag.Benefits = Session["Benefitsterms"].ToString();
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
            if (Session["BenefitstermsVer"] == null && Session["PolicytermsVer"] == null)
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
                            TempData["irent_access_token"] = Session["irent_access_token"];
                            return RedirectToRoute(new
                            {
                                controller = "HotaiPayCtbc",
                                action = "NoCreditCard"
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
                        memberBenefitsVersion = Session["BenefitstermsVer"] == null ? "" : Session["BenefitstermsVer"].ToString(),
                        privacyPolicyVersion =Session["PolicytermsVer"]==null?"": Session["PolicytermsVer"].ToString()
                    };
                    WebAPIOutput_BenefitsAndPrivacyVersion output = new WebAPIOutput_BenefitsAndPrivacyVersion();
                    flag = hotaiAPI.DoUpdateBenefitsAndPrivacyVersion(Session["hotai_access_token"].ToString().Trim(), input, ref output, ref errCode);
                    //flag = true;
                    if (flag)
                    {
                        TempData["irent_access_token"] = Session["irent_access_token"];
                        return RedirectToRoute(new
                        {
                            controller = "HotaiPayCtbc",
                            action = "NoCreditCard"
                        });
                    }
                    else
                        return RedirectToRoute(new { controller = "HotaiPay", action = "MembershipTerms1" });
                }
                else
                    return RedirectToRoute(new { controller = "HotaiPay", action = "RegisterStep1" });
            }
        }

        public ActionResult MembershipTerms1()
        {
            ViewBag.Privacy = Session["Policyterms"];
            return View();
        }

        #endregion

        #region 補填會員資料頁面
        public ActionResult Supplememtary()
        {
            if (Session["hotai_access_token"] != null)
            {
                ViewBag.CustID = Session["id"];
                ViewBag.Name = Session["name"];
                ViewBag.Birthday = Session["birth"];
                ViewBag.Email = Session["email"];
            }

            return View();
        }

        [HttpPost]
        public ActionResult Supplememtary(SignUpProfile signUpProfile)
        {
            bool flag = true;
            string errCode = "";

            if (signUpProfile.Sex == "male")
            {
                ViewBag.MaleCheck = true;
                ViewBag.FemaleCheck = false;
            }
            else if (signUpProfile.Sex == "female")
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = true;
            }
            else
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = false;
            }

            if (ModelState.IsValid)
            {


                if (!CheckIdno(signUpProfile.CustID.Trim()))
                {
                    ViewBag.CustIDAlert = "身分證格式錯誤";
                    return View("Supplememtary");
                }

                else
                {
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
                                    action = "MembershipTerms2",
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
                                        TempData["irent_access_token"] = Session["irent_access_token"];
                                        return RedirectToRoute(new
                                        {
                                            controller = "HotaiPayCtbc",
                                            action = "NoCreditCard"
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
                        return View("Supplememtary");
                    }
                }
            }
            else
            {
                if (ModelState.IsValidField("Name"))
                {
                    ViewBag.Name = Session["name"];
                }
                if (ModelState.IsValidField("CustID"))
                {
                    ViewBag.CustID = Session["id"];
                }
                if (ModelState.IsValidField("Birth"))
                {
                    ViewBag.Birthday = Session["birth"];
                }
                if (ModelState.IsValidField("Email"))
                {
                    ViewBag.Email = Session["email"];
                }
                return View(signUpProfile);
            }

        }
        #endregion

        #region 註冊驗證步驟一:手機驗證
        public ActionResult RegisterStep1()
        {
            ViewBag.Phone = Session["Phone"];
            ViewBag.OtpCode = Session["OtpCode"];
            ViewBag.PhoneAlert = Session["AlertPhone"];
            ViewBag.OtpAlert = Session["AlertOtp"];
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
                Session["AlertPhone"] = "此帳號已被註冊";
                return RedirectToAction("RegisterStep1");
            }
            else
            {
                ViewBag.PhoneAlert = "";
                ViewBag.OtpAlert = "";
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
                    Session["AlertPhone"] = "已成功發送驗證碼，請留意手機簡訊";
                    return Redirect("RegisterStep1");
                }
                else
                {
                    Session["AlertOtp"] = errorDic[errCode];

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
                    ViewBag.Alert = "驗證碼錯誤，請重新輸入。";
                    return View();
                }
            }
            catch (Exception e)
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
            ViewBag.CustID = Session["id"];
            ViewBag.Name = Session["name"];
            ViewBag.Birthday = Session["birth"];
            ViewBag.Email = Session["email"];
            return View();
        }

        [HttpPost]
        public ActionResult SetSignUpProfile(SignUpProfile signUpProfile)
        {
            ViewBag.CustID = signUpProfile.CustID;
            ViewBag.Name = signUpProfile.Name;
            ViewBag.Birthday = signUpProfile.Birth;
            ViewBag.Email = signUpProfile.Email;

            if (signUpProfile.Sex == "male")
            {
                ViewBag.MaleCheck = true;
                ViewBag.FemaleCheck = false;
            }
            else if (signUpProfile.Sex == "female")
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = true;
            }
            else
            {
                ViewBag.MaleCheck = false;
                ViewBag.FemaleCheck = false;
            }

            if (ModelState.IsValid)
            {
                bool flag = false;
                string errCode = "";

                if (!CheckIdno(signUpProfile.CustID.Trim()))
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
                        {
                            TempData["irent_access_token"] = Session["irent_access_token"];
                            return RedirectToRoute(new
                            {
                                controller = "HotaiPayCtbc",
                                action = "CreditStart"
                            });
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
            //return RedirectToAction("/Login", new { irent_access_token = Session["irent_access_token"], phone = Session["phone"] });
        }
        [HttpPost]
        public ActionResult BindCardFailed(string mode)
        {
            logger.Info($"tanginput_BindCardFailed : {Session["p"]}");
            //return RedirectToAction("/Login", new { p = Session["p"]} );
            //return RedirectToAction($"/Login?p={Session["p"]}");

            //20220117唐改
            //return RedirectToAction("Login", "HotaiPay", new { p = Session["p"] });
            return RedirectToRoute(new { controller = "HotaiPayCtbc", action = "NoCreditCard" });
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
            string encryptStr = Request.QueryString["p"] != null ? (Request.QueryString["p"].Trim()) : "";
            if (!string.IsNullOrWhiteSpace(encryptStr))
            {
                var decryptDic = HPServices.QueryStringDecryption(encryptStr);
                if (!string.IsNullOrWhiteSpace(decryptDic["irent_access_token"]))
                {
                    ViewData["Token"] = decryptDic["irent_access_token"];
                    return View();
                }
                else
                {
                    return RedirectToAction("MemberFail");
                }
            }
            else
            {
                return RedirectToAction("MemberFail");
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

            if (flag)
            {
                SPInput_MemberUnBind sp_unBindinput = new SPInput_MemberUnBind() { IDNO = IDNO, PRGName = "Unbind" };
                SPOutput_MemberUnBind spOutput = new SPOutput_MemberUnBind();
                flag = HPServices.sp_MemberUnBind(sp_unBindinput, ref spOutput, ref errCode);

                if (spOutput != null)
                {
                    //20220112 和泰要求解綁打登出API 使RefreshToken失效
                    WebAPIInput_SignOut signOut = new WebAPIInput_SignOut()
                    {
                        refresh_token = spOutput.RefreshToken
                    };
                    hotaiAPI.DoSignOut(spOutput.AccessToken, signOut, ref errCode);
                }
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

        #region 和泰會員進入失敗
        public ActionResult MemberFail()
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

        public string InsertMemberDataToDB(string id, string oneID, string accessToken, string refreshToken)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
            var logID = 999;
            using (SqlConnection conn = new SqlConnection(connectionString))
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

        //public bool CheckROCID(string idNo)
        //{
        //    if (idNo == null)
        //    {
        //        return false;
        //    }
        //    idNo = idNo.ToUpper();
        //    Regex regex = new Regex(@"^([A-Z])([1-2]\d{8})$");
        //    Match match = regex.Match(idNo);
        //    if (!match.Success)
        //    {
        //        return false;
        //    }

        //    ///建立字母對應表(A~Z)
        //    ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
        //    ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35 
        //    string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
        //    string transferIdNo = $"{(alphabet.IndexOf(match.Groups[1].Value) + 10)}" +
        //                          $"{match.Groups[2].Value}";
        //    int[] idNoArray = transferIdNo.ToCharArray()
        //                                  .Select(c => Convert.ToInt32(c.ToString()))
        //                                  .ToArray();
        //    int sum = idNoArray[0];
        //    int[] weight = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
        //    for (int i = 0; i < weight.Length; i++)
        //    {
        //        sum += weight[i] * idNoArray[i + 1];
        //    }
        //    return (sum % 10 == 0);
        //}

        /// <summary>
        /// 判斷身分證號及統一證號是否正確，並判斷性別及國籍
        ///  20210830修改 外籍人士身分證號比照國民身分證號 第二碼8為男 第二碼9為女
        /// 國籍
        /// 本署核發之外來人口統一證號編碼，共計10碼，前2碼使用英文字母，
        ///第1碼為區域碼（同國民身分證註1）
        ///第2碼為性別碼(註 2)、3至10碼為阿拉伯數字，其中第3至9碼為流水號、第10碼為檢查號碼。
        ///註1：英文字母代表直轄市、縣、市別：
        /// 台北市 A、台中市 B、基隆市 C、台南市 D、高雄市 E
        /// 新北市 F、宜蘭縣 G、桃園縣 H、嘉義市 I、新竹縣 J
        /// 苗栗縣 K、原台中縣 L、南投縣 M、彰化縣 N、新竹市 O
        /// 雲林縣 P、嘉義縣 Q、原台南縣 R、原高雄縣 S、屏東縣 T
        /// 花蓮縣 U、台東縣 V、金門縣 W、澎湖縣 X、連江縣 Z
        /// 註2：
        /// 臺灣地區無戶籍國民、大陸地區人民、港澳居民：
        /// 男性使用A、女性使用B
        ///外國人：
        /// 男性使用C、女性使用D

        /// 第二碼8為男 第二碼9為女
        /// </summary>
        /// <param name="str"></param>
        public bool CheckIdno(String idno)
        {
            string sex = "";
            string nationality = "";
            if (idno == null || string.IsNullOrWhiteSpace(idno) || idno.Length != 10)
            {
                return false;
            }
            char[] pidCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            idno = idno.ToUpper(); // 轉換大寫
            char[] strArr = idno.ToCharArray(); // 字串轉成char陣列
            int verifyNum = 0;

            // string pat = @"[A-Z]{1}[1-2]{1}[0-9]{8}";
            string pat = @"[A-Z]{1}[1289]{1}[0-9]{8}";   // 20210830修改 外籍人士身分證號比照國民身分證號 第二碼8為男 第二碼9為女
                                                         // Instantiate the regular expression object.
            Regex rTaiwan = new Regex(pat, RegexOptions.IgnoreCase);
            // Match the regular expression pattern against a text string.
            Match mTaiwan = rTaiwan.Match(idno);
            // 檢查身分證字號
            if (mTaiwan.Success)
            {
                // 原身分證英文字應轉換為10~33，這裡直接作個位數*9+10
                int[] pidIDInt = { 1, 10, 19, 28, 37, 46, 55, 64, 39, 73, 82, 2, 11, 20, 48, 29, 38, 47, 56, 65, 74, 83, 21, 3, 12, 30 };
                // 第一碼
                verifyNum = verifyNum + pidIDInt[Array.BinarySearch(pidCharArray, strArr[0])];
                // 第二~九碼
                for (int i = 1, j = 8; i < 9; i++, j--)
                {
                    verifyNum += Convert.ToInt32(strArr[i].ToString(), 10) * j;
                }
                // 檢查碼
                verifyNum = (10 - (verifyNum % 10)) % 10;
                bool ok = verifyNum == Convert.ToInt32(strArr[9].ToString(), 10);
                if (ok)
                {
                    // 判斷性別 & 國籍
                    sex = "男";
                    if (strArr[1] == '2' || strArr[1] == '9') sex = "女";  // 20210830 修改
                    nationality = "本國籍";

                    if (strArr[1] == '8' || strArr[1] == '9') nationality = "外籍人士";  // 20210830 修改
                }
                return ok;
            }

            // 檢查統一證號(居留證)
            verifyNum = 0;
            pat = @"[A-Z]{1}[A-D]{1}[0-9]{8}";
            // Instantiate the regular expression object.
            Regex rForeign = new Regex(pat, RegexOptions.IgnoreCase);
            // Match the regular expression pattern against a text string.
            Match mForeign = rForeign.Match(idno);
            if (mForeign.Success)
            {
                // 原居留證第一碼英文字應轉換為10~33，十位數*1，個位數*9，這裡直接作[(十位數*1) mod 10] + [(個位數*9) mod 10]
                int[] pidResidentFirstInt = { 1, 10, 9, 8, 7, 6, 5, 4, 9, 3, 2, 2, 11, 10, 8, 9, 8, 7, 6, 5, 4, 3, 11, 3, 12, 10 };
                // 第一碼
                verifyNum += pidResidentFirstInt[Array.BinarySearch(pidCharArray, strArr[0])];
                // 原居留證第二碼英文字應轉換為10~33，並僅取個位數*6，這裡直接取[(個位數*6) mod 10]
                int[] pidResidentSecondInt = { 0, 8, 6, 4, 2, 0, 8, 6, 2, 4, 2, 0, 8, 6, 0, 4, 2, 0, 8, 6, 4, 2, 6, 0, 8, 4 };
                // 第二碼
                verifyNum += pidResidentSecondInt[Array.BinarySearch(pidCharArray, strArr[1])];
                // 第三~八碼
                for (int i = 2, j = 7; i < 9; i++, j--)
                {
                    verifyNum += Convert.ToInt32(strArr[i].ToString(), 10) * j;
                }
                // 檢查碼
                verifyNum = (10 - (verifyNum % 10)) % 10;
                bool ok = verifyNum == Convert.ToInt32(strArr[9].ToString(), 10);
                if (ok)
                {
                    // 判斷性別 & 國籍
                    sex = "男";
                    if (strArr[1] == 'B' || strArr[1] == 'D') sex = "女";
                    nationality = "外籍人士";
                    if (strArr[1] == 'A' || strArr[1] == 'B') nationality += "(臺灣地區無戶籍國民、大陸地區人民、港澳居民)";
                }
                return ok;
            }
            return false;
        }
    }
}