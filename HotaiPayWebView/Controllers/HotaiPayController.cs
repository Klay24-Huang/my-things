using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output.Hotai.Payment;
using HotaiPayWebView.Repository;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCommon;
using NLog;
using WebCommon;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayController : Controller
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
        #region 登入頁面
        public ActionResult Login()
        {
            ViewBag.phone = Request.QueryString["phone"];
            TempData["phone"] = Request.QueryString["phone"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(string phone, string pwd)
        {

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
                                TempData["trems"] += checkVer.memberBenefits;
                            if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                TempData["trems"] += checkVer.privacyPolicy;
                            return View("MembershipTerms");
                        }
                        else
                        {
                            WebAPIOutput_GetMobilePhoneToOneID getOneID = new WebAPIOutput_GetMobilePhoneToOneID();
                            flag = hotaiAPI.DoGetMobilePhoneToOneID(phone, ref getOneID, ref errCode);
                            if (flag)
                            {
                                TempData["oneID"] = getOneID.memberSeq;

                                //以下取得信用卡列表流程
                            }
                            else
                            {
                                RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
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
                }
            }
            RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
            return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" });
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

        #region 註冊驗證步驟一:手機驗證
        public ActionResult RegisterStep1()
        {

            return View();
        }
        [HttpPost]
        public ActionResult GetOtpCode(string phone)
        {
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
                ViewBag.Alert = "此帳號已被註冊";
                return View();
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
                    return View();
                }
                else
                {
                    ViewBag.Alert = "驗證碼傳送失敗，請再試一次";
                    return View();
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
                return View("RegisterStep2");
            }
            else
            {

            }
            return View();

        }
        #endregion

        #region 註冊驗證步驟二:密碼設定
        public ActionResult RegisterStep2()
        {
            return View();
        }
        #endregion

        #region 註冊驗證步驟三:會員資料填寫
        public ActionResult RegisterStep3()
        {
            return View();
        }
        #endregion

        #region 綁卡失敗
        public ActionResult BindCardFailed()
        {
            return View("BindCardFailed");
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
            HotaiPaymentAPI hotaiPayAPI = new HotaiPaymentAPI();
            bool flag = false;
            string errCode = "";
            WebAPIInput_GetCreditCards apiInput = new WebAPIInput_GetCreditCards();//TODO不用指定值?
            WebAPIOutput_GetCreditCards apioutput = new WebAPIOutput_GetCreditCards();

            flag = string.IsNullOrWhiteSpace(HCToken);
            //token檢核
            flag = hotaiMemAPI.DoCheckToken(HCToken, ref errCode);
            if (!flag)
            {
                //TODO Token失效 導URL至登入畫面 請使用者重登
                return View("Login");
            }
            WebAPIInput_GetCreditCards CardsListinput = new WebAPIInput_GetCreditCards();
            WebAPIOutput_GetCreditCards CardsListoutput = new WebAPIOutput_GetCreditCards();
            flag = hotaiPayAPI.GetHotaiCardList(CardsListinput, ref CardsListoutput);
            if (!flag)
            {
                //TODO 取得綁卡清單
                //TODO 待確認是callAPI 失敗還是API回傳失敗
            }
            else
            {
                //TODO
                if (CardsListoutput.CardCount > 0)
                {
                    return View("CreditCardChoose");
                    //call Java Script 調整畫面上資料

                }//else 停留在目前畫面(no-creditcard 新增綁卡)
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