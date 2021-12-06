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

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayController : Controller
    {
        private static string _accessToken = "";
        private static string _refreshToken = "";

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
        private static readonly Dictionary<string, string> errorDic = commonRepository.GetErrorList("").ToLookup(x => x.ErrCode, y => y.ErrMsg).ToDictionary(x => x.Key, y => y.First());
        static HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
        private static HashAlgorithmHelper helper = new HashAlgorithmHelper();
        #region 登入頁面
        public ActionResult Login()
        {
            ViewBag.phone = Request.QueryString["phone"];
            TempData["phone"] = Request.QueryString["phone"];
            return View();
            //return RedirectToAction("CreditCardChoose", "HotaiPayCtbc");
        }

        public void RefreshToken(ref string accessToken, ref string refreshToken)
        {
            bool flag = false;
            WebAPIInput_RefreshToken tokenInput = new WebAPIInput_RefreshToken
            {
                access_token = _accessToken,
                refresh_token = _refreshToken
            };
            WebAPIOutput_Token tokenOutput = new WebAPIOutput_Token();
            string errCode = "";
            int a = 404;
            flag = hotaiAPI.DoRefreshToken(tokenInput, ref tokenOutput, ref errCode, ref a);

            if (flag)
            {
                _accessToken = tokenOutput.access_token;
                _refreshToken = tokenOutput.refresh_token;
            }
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
                TempData["token"] = apioutput.access_token;
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
                                TempData["terms"] += checkVer.memberBenefits;
                            if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                TempData["terms"] += checkVer.privacyPolicy;
                            return View("MembershipTerms");
                        }
                        else
                        {
                            WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                            flag = hotaiAPI.DoGetMobilePhoneToOneID(phone, ref getOneID, ref errCode);
                            if (flag)
                            {
                                TempData["oneID"] = getOneID.memberSeq;
                                return RedirectToAction("CreditStart");
                                //以下取得信用卡列表流程
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
                if (errCode == "ERR980" || errCode == "ERR953")
                {
                    this.TempData["MSG"] = "密碼錯誤";
                    return RedirectToAction("BindCardFailed");
                }
            }

            //RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
            //return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
            return RedirectToAction("BindCardFailed");

        }
        #endregion


        #region 更新會員條款
        public ActionResult MembershipTerms()
        {
            return View();
        }
        #endregion

        #region 補填會員資料頁面
        public ActionResult Supplememtary()
        {
            return View();
        }
        #endregion

        [Route("~/HotaiPay/RegisterStep1")]
        #region 註冊驗證步驟一:手機驗證
        public ActionResult RegisterStep1()
        {
            ViewBag.Phone = Session["Phone"];
            ViewBag.OtpCode = Session["OtpCode"];
            ViewBag.Alert = Session["Alert"];
            return View();
        }

        [HttpPost]
        public RedirectResult GetOtpCode(string phone)
        {
            TempData["Phone"] = phone;
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

                    return Redirect("RegisterStep2");
                }
            }
        }
        [HttpPost]
        public ActionResult CheckOtpCode(string phone, string otpCode)
        {


            bool flag = false;
            WebAPIInput_SmsOtpValidation checkSMSOpt = new WebAPIInput_SmsOtpValidation
            {
                mobilePhone = phone,
                otpCode = otpCode,
                useType = 1
            };
            WebAPIOutput_OtpValidation checkSMSOptOutput = new WebAPIOutput_OtpValidation();
            string errCode = "";
            flag = hotaiAPI.DoSmsOtpValidation(checkSMSOpt, ref checkSMSOptOutput, ref errCode);

            if (flag)
            {
                Session["Phone"] = phone;
                Session["OtpCode"] = otpCode;
                Session["OtpID"] = checkSMSOptOutput.otpId;
                return RedirectToAction("RegisterStep2", "HotaiPay");
            }
            else
            {
                return RedirectToAction("RegisterStep1", "HotaiPay");
            }

        }
        #endregion

        #region 註冊驗證步驟二:密碼設定
        [Route("~/Home/RegisterStep2")]
        public ActionResult RegisterStep2()
        {
            var phone = Session["Phone"].ToString();
            return View();
        }

        [HttpPost]
        public ActionResult DoSignUp(string pwd, string comfirmPwd)
        {
            var phone = TempData["Phone"].ToString();
            var otpID = TempData["OtpID"].ToString();
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
                _accessToken = getToken.access_token;
                _refreshToken = getToken.refresh_token;
            }
            return View();
        }
        #endregion

        #region 註冊驗證步驟三:會員資料填寫
        public ActionResult RegisterStep3()
        {
            var phone = TempData["Phone"].ToString();

            return View();
        }
        public ActionResult SetSignUpProfile(WebAPIInput_SignupProfile memberProfileInput)
        {
            bool flag = false;
            string errCode = "";

            flag = hotaiAPI.DoSignupProfile(_accessToken, memberProfileInput, ref errCode);

            if (flag)
            {

            }
            else
            {

            }
            return View();
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
            return View();
        }
        #endregion

        #region 無信用卡列表頁面 
        public ActionResult NoCreditCard(string HCToken)
        {
            HotaiMemberAPI hotaiMemAPI = new HotaiMemberAPI();
            bool flag = false;
            string errCode = "";

            flag = string.IsNullOrWhiteSpace(HCToken);
            //token檢核
            int a = 404;
            flag = hotaiMemAPI.DoCheckToken(HCToken, ref errCode, ref a);

            /*if (!flag)

            {
                //TODO Token失效 導URL至登入畫面 請使用者重登
                return View("Login");
            }*/
            HotaiPaymentAPI HPAPI = new HotaiPaymentAPI();
            HotaipayService HPServices = new HotaipayService();
            //取得卡片清單
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            //設定查詢的IDNO
            input.IDNO = "F128697972";//測試用資料 上線需更改
            flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
            if (flag)
            {
                if (output.CreditCards.Count > 0)
                { //TODO 跳轉卡片清單畫面
                    return View("CreditCardChoose");
                    //call Java Script 調整畫面上資料?
                }
            }
            else
            { //TODO API回傳失敗
            }
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
        public ActionResult NoBind()
        {
            return View();
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


    }
}