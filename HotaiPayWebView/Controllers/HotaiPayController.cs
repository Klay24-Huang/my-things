using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using HotaiPayWebView.Repository;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayController : Controller
    {
        HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
        #region 登入頁面
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoLogin(string phone,string pwd)
        {
            bool flag = false;
            string errCode = "";

            WebAPIInput_Signin apiInput = new WebAPIInput_Signin
            {
                account = phone,
                password = pwd
            };

            WebAPIOutput_Signin apioutput = new WebAPIOutput_Signin();

            
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
                                return View("BindCardFailed");
                            }
                        }
                    }   
                }
                    
            }
            else {
                if (errCode=="ERR980")
                {
                    ViewBag.MSG = "密碼錯誤";
                }
            }
            return View();
        }
        #endregion

        #region 會員條款
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
        public ActionResult NoCreditCard()
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

        #region 註冊成功會員條款同意頁面
        public ActionResult RegisterMembershipTerm()
        {
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