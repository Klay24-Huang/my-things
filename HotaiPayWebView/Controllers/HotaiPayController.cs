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
            Session["phone"] = Request.QueryString["phone"];
            Session["id"] = Request.QueryString["id"];
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
                Session["token"] = apioutput.access_token;
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
                                Session["terms"] += checkVer.memberBenefits;
                            if (string.IsNullOrEmpty(checkVer.privacyPolicyVersion))
                                Session["terms"] += checkVer.privacyPolicy;
                            return View("MembershipTerms");
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
                                    //以下取得信用卡列表流程
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
                if (errCode == "ERR980" || errCode == "ERR953")
                {
                    this.TempData["MSG"] = "密碼錯誤";
                    return RedirectToAction("BindCardFailed");
                }
            }
            return RedirectToRoute(new { controller = "HotaiPay", action = "AlreadyMember" });
        }
        #endregion

        #region 更新會員條款
        public ActionResult MembershipTerms1()
        {
            return View();
        }
        public ActionResult MembershipTerms2()
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
        [Route("~/HotaiPay/RegisterStep1")]
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
                _accessToken = getToken.access_token;
                _refreshToken = getToken.refresh_token;
            }
            return View();
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

            flag = hotaiAPI.DoSignupProfile(_accessToken, memberProfileInput, ref errCode);

            if (flag)
            {
                return Redirect("RegisterSuccess");
            }
            else
            {
                return View("RegisterStep3");
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
            int a = 404;//測試用資料 上線需更改
            flag = hotaiMemAPI.DoCheckToken(HCToken, ref errCode, ref a);

            /*if (!flag)
            {
                //TODO Token失效 導URL至登入畫面 請使用者重登
                return View("Login");
            }*/
            HotaipayService HPServices = new HotaipayService();
            //取得卡片清單
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            //設定查詢的IDNO
            input.IDNO = "C221120413";//測試用資料 上線需更改
            flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
           
            if (flag)
            {
                if (output.CreditCards.Count > 0)
                { //TODO 跳轉卡片清單畫面

                    List<HotaiCardInfo> L_Output = output.CreditCards;
                    
                    return View("CreditCardChoose", L_Output);
                }
            }
            else { //TODO API回傳失敗

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
        public ActionResult AlreadyMember()
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