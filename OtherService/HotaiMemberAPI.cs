using Domain.SP.Input.OtherService.Common;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebCommon;


namespace OtherService
{
    /// <summary>
    /// 和泰會員
    /// </summary>
    public class HotaiMemberAPI
    {
        private static ConfigManager configManager = new ConfigManager("hotaipayment");
        private string FrontEndURL = configManager.GetKey("HotaiMemberFrontEndURL");
        private string SingleEntry = configManager.GetKey("HotaiMemberSingleEntry");
        private string BackEndURL = configManager.GetKey("HotaiMemberBackEndURL");
        private string AppId = configManager.GetKey("HotaiAppId");
        private string AppVersion = configManager.GetKey("HotaiAppVersion");
        private string ApiVersion = configManager.GetKey("HotaiApiVersion");
        private string AppKey = configManager.GetKey("HotaiAppKey");
        private string Key = configManager.GetKey("HotaiKey");
        private string IV = configManager.GetKey("HotaiIV");

        private string CheckSignupURL = "api/signup/check";                                                //註冊檢查  
        private string SendSmsOtpURL = "api/otp/sms";                                                      //發送簡訊OTP
        private string SmsOtpValidationURL = "api/otp/sms-validatation";                                   //簡訊 OTP 驗證
        private string RefreshTokenURL = "api/token/refresh";                                              //更新 Token
        private string SendEmailOtpURL = "api/otp/email";                                                  //發送 Email OTP
        private string EmailOtpValidatationURL = "api/otp/email-validatation";                             //Email OTP 驗證
        private string CheckTokenURL = "api/token/check";                                                  //檢查 Token
        private string GetEmailURL = "api/otp/sms-email";                                                  //取得 Email
        private string OtpValidatationURL = "api/otp/validatation";                                        //驗證會員資訊,取得 OTP 編號
        private string SignOutURL = "api/signOut";                                                         //登出
        private string SigninURL = "api/signin";                                                           //登入
        private string SignupURL = "api/signup";                                                           //註冊
        private string SignupProfileURL = "api/member/signup-profile";                                     //註冊個人資料(一般)
        private string ResetPasswordURL = "api/member/reset-password";                                     //重設密碼
        private string IsMissingMemberProfileURL = "api/member/missing-profile";                           //缺少個人資料
        private string GetMemberProfileURL = "api/member/profile";                                         //取得個人資料
        private string UpdateMemberProfileURL = "api/member/update-profile";                               //更新個人資料
        private string ConfirmPasswordURL = "api/member/confirm-password";                                 //確認密碼
        private string UpdateAccountURL = "api/member/update-account";                                     //修改帳號
        private string UpdatePasswordURL = "api/member/update-password";                                   //修改密碼
        private string CheckBenefitsAndPrivacyVersionURL = "api/member/check-benefits-privacy-version";    //檢查會員權益及隱私條款版本
        private string UpdateBenefitsAndPrivacyVersionURL = "api/member/update-benefits-privacy-version";  //同意新款會員權益及隱私條款
        private string GetPrivacyURL = "api/privacy";                                                      //取得會員權益及隱私條款
        private string GroupAppsURL = "api/group-apps/";                                                   //集團服務

        private string GetMobilePhoneToOneIDURL = "api/subsystem/member/MobilePhoneToOneID";               //【後台】使用手機取得會員 OneID
        private string GetTownshipsURL = "api/subsystem/townships";                                        //【後台】行政區列表
        private string GetPublicKeyURL = "api/subsystem/PublicKEY";                                        //【後台】取得公鑰    
        private string GetValidKeyVersionURL = "api/subsystem/valid-key-version";                          //【後台】取得後台金鑰有效版本號    
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 發送簡訊 OTP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSendSmsOtp(WebAPIInput_SendSmsOtp input, ref WebAPIOutput_SendSmsOtp output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_SendSmsOtp>(input, SendSmsOtpURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_SendSmsOtp>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 簡訊 OTP 驗證
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSmsOtpValidation(WebAPIInput_SmsOtpValidation input, ref WebAPIOutput_OtpValidation output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_SmsOtpValidation>(input, SmsOtpValidationURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_OtpValidation>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 更新 Token
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoRefreshToken(WebAPIInput_RefreshToken input, ref WebAPIOutput_Token output, ref string errCode, ref int HttpStatusCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_RefreshToken>(input, RefreshTokenURL, ref errCode, MethodBase.GetCurrentMethod().Name);
            HttpStatusCode = result.HttpStatusCode;
            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_Token>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 發送 Email OTP
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSendEmailOtp(string token, WebAPIInput_SendEmailOtp input, ref WebAPIOutput_SendSmsOtp output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_SendEmailOtp>(input, SendEmailOtpURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_SendSmsOtp>(result.Data.ToString());
            }
            return flag;

        }

        /// <summary>
        /// Email OTP 驗證
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoEmailOtpValidatation(string token, WebAPIInput_EmailOtpValidatation input, ref WebAPIOutput_OtpValidation output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_EmailOtpValidatation>(input, EmailOtpValidatationURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_OtpValidation>(result.Data.ToString());
            }
            return flag;

        }

        /// <summary>
        /// 檢查 Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoCheckToken(string token, ref string errCode, ref int HttpStatusCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, object>(null, CheckTokenURL, ref errCode, MethodBase.GetCurrentMethod().Name, token, "GET");
            HttpStatusCode = result.HttpStatusCode;
            if (result.Succ)
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 取得 Email
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetEmail(WebAPIInput_GetEmail input, ref WebAPIOutput_GetEmail output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_GetEmail>(input, GetEmailURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_GetEmail>(result.Data.ToString());
            }
            return flag;

        }

        /// <summary>
        /// 驗證會員資訊，取得 OTP 編號
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoOtpValidatation(WebAPIInput_OtpValidatation input, ref WebAPIOutput_OtpValidation output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_OtpValidatation>(input, OtpValidatationURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_OtpValidation>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSignOut(string token, WebAPIInput_SignOut input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_SignOut>(input, SignOutURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSignin(WebAPIInput_Signin input, ref WebAPIOutput_Signin output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_Signin>(input, SigninURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_Signin>(result.Data.ToString());
            }

            return flag;
        }

        /// <summary>
        /// 註冊檢查
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns> 
        public bool DoCheckSignup(WebAPIInput_CheckSignup input, ref WebAPIOutput_CheckSignup output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_CheckSignup>(input, CheckSignupURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_CheckSignup>(result.Data.ToString());
            }

            return flag;
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSignup(WebAPIInput_Signup input, ref WebAPIOutput_Token output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_Signup>(input, SignupURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_Token>(result.Data.ToString());
            }

            return flag;
        }

        /// <summary>
        /// 註冊個人資料(一般)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoSignupProfile(string token, WebAPIInput_SignupProfile input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_SignupProfile>(input, SignupProfileURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;
        }


        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoResetPassword(WebAPIInput_ResetPassword input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_ResetPassword>(input, ResetPasswordURL, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 缺少個人資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoIsMissingMemberProfile(string token, ref WebAPIOutput_IsMissingMemberProfile output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, object>(null, IsMissingMemberProfileURL, ref errCode, MethodBase.GetCurrentMethod().Name, token, "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_IsMissingMemberProfile>(result.Data.ToString());
            }

            return flag;
        }

        /// <summary>
        /// 取得個人資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetMemberProfile(string token, ref WebAPIOutput_GetMemberProfile output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, object>(null, GetMemberProfileURL, ref errCode, MethodBase.GetCurrentMethod().Name, token, "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_GetMemberProfile>(result.Data.ToString());
            }

            return flag;
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoUpdateMemberProfile(string token, WebAPIInput_UpdateMemberProfile input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_UpdateMemberProfile>(input, UpdateMemberProfileURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;

        }

        /// <summary>
        /// 確認密碼
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoConfirmPassword(string token, WebAPIInput_ConfirmPassword input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_ConfirmPassword>(input, ConfirmPasswordURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 修改帳號
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoUpdateAccount(string token, WebAPIInput_UpdateAccount input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_UpdateAccount>(input, UpdateAccountURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoUpdatePassword(string token, WebAPIInput_UpdatePassword input, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_UpdatePassword>(input, UpdatePasswordURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 檢查會員權益及隱私條款版本
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoCheckBenefitsAndPrivacyVersion(string token, ref WebAPIOutput_BenefitsAndPrivacyVersion output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, object>(null, CheckBenefitsAndPrivacyVersionURL, ref errCode, MethodBase.GetCurrentMethod().Name, token, "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_BenefitsAndPrivacyVersion>(result.Data.ToString());
            }

            return flag;

        }

        /// <summary>
        /// 同意新款會員權益及隱私條款
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoUpdateBenefitsAndPrivacyVersion(string token, WebAPIInput_UpdateBenefitsAndPrivacyVersion input, ref WebAPIOutput_BenefitsAndPrivacyVersion output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, WebAPIInput_UpdateBenefitsAndPrivacyVersion>(input, UpdateBenefitsAndPrivacyVersionURL, ref errCode, MethodBase.GetCurrentMethod().Name, token);

            if (result.Succ)
            {
                flag = true;
                //output = JsonConvert.DeserializeObject<WebAPIOutput_BenefitsAndPrivacyVersion>(result.Data.ToString()); 和泰沒回ResponseData
            }
            return flag;
        }

        /// <summary>
        /// 取得會員權益及隱私條款
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetPrivacy(string token, ref WebAPIOutput_GetPrivacy output, ref string errCode)
        {
            bool flag = false;
            var result = HotaiMemeberApiPost<object, object>(null, GetPrivacyURL, ref errCode, MethodBase.GetCurrentMethod().Name, token, "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_GetPrivacy>(result.Data.ToString());
            }
            return flag;

        }

        /// <summary>
        /// 集團服務
        /// </summary>
        /// <param name="deviceOS">裝置作業系統</param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGroupApps(WebAPIInput_GroupApps input, ref WebAPIOutput_GroupApps output, ref string errCode)
        {
            string device = "";
            switch (input.deviceOS)
            {
                case 1:
                    device = "ios";
                    break;
                case 2:
                    device = "android";
                    break;
            }

            bool flag = false;
            string apiUrl = $"{GroupAppsURL}{device}";
            var result = HotaiMemeberApiPost<object, WebAPIOutput_GroupApps>(null, apiUrl, ref errCode, MethodBase.GetCurrentMethod().Name, "GET", "GET");

            if (result.Succ)
            {
                flag = true;
                var resData = JsonConvert.DeserializeObject<List<GroupApps>>(result.Data.ToString());
                output.groupApps = resData;
            }
            return flag;
        }

        /// <summary>
        /// 【後台】使用手機取得OneID
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGetMobilePhoneToOneID(string mobilePhone, ref WebAPIOutput_GetMobilePhoneToOneID output, ref string errCode)
        {
            bool flag = false;
            string API = $"{GetMobilePhoneToOneIDURL}/{mobilePhone}";

            var result = HotaiMemeberBackendApiPost<object, object>(null, API, ref errCode, MethodBase.GetCurrentMethod().Name, "", "GET", "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_GetMobilePhoneToOneID>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 【後台】行政區列表
        /// </summary>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetTownships(ref WebAPIOutput_Townships output, ref string errCode)
        {
            bool flag = false;
            string API = $"{GetTownshipsURL}";

            var result = HotaiMemeberBackendApiPost<object, object>(null, API, ref errCode, "", "GET", "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_Townships>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 【後台】取得公鑰
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetPublicKey(WebAPIInput_GetPublicKey input, ref WebAPIOutput_GetPublicKey output, ref string errCode)
        {
            bool flag = false;
            string type = "O";
            switch (input.type)
            {
                case 0:
                    type = "O";
                    break;
                case 1:
                    type = "N";
                    break;
            }

            string API = $"{GetPublicKeyURL}/{type}";

            var result = HotaiMemeberBackendApiPost<object, object>(null, API, ref errCode, MethodBase.GetCurrentMethod().Name, "", "GET", "GET");

            if (result.Succ)
            {
                flag = true;
                output = JsonConvert.DeserializeObject<WebAPIOutput_GetPublicKey>(result.Data.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 【後台】取得後台金鑰有效版本號
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool DoGetValidKeyVersion(WebAPIInput_GetValidKeyVersion input, ref WebAPIOutput_GetValidKeyVersion output, ref string errCode)
        {
            bool flag = false;
            string API = $"{GetValidKeyVersionURL}";

            var result = HotaiMemeberBackendApiPost<object, WebAPIInput_GetValidKeyVersion>(input, API, ref errCode, MethodBase.GetCurrentMethod().Name);

            if (result.Succ)
            {
                flag = true;
                var data = JsonConvert.DeserializeObject<List<Data>>(result.Data.ToString());
                output.Data = data;
            }
            return flag;
        }

        private (bool Succ, string Message, int HttpStatusCode, TResponse Data)
            HotaiMemeberApiPost<TResponse, TRequest>(TRequest Body, string API, ref string errCode, string funName, string access_token = "", string Action = "POST", string Method = "POST")
        {
            (bool Succ, string Message, int HttpStatusCode, TResponse Data) valueTuple =
                (false, "", 0, default(TResponse));

            string BaseUrl = FrontEndURL;
            string api = SingleEntry;
            var requestUrl = $"{BaseUrl}{api}";
            string error = "";

            try
            {
                var header = SetRequestHeader(access_token);

                var body = Body == null ? "" : JsonConvert.SerializeObject(Body);

                var resinfo = SetRequestBody(body, API, Action);

                string content = JsonConvert.SerializeObject(resinfo);

                logger.Info($"Post Body:{content}");

                var result = ApiPost.DoApiPostJson(requestUrl, content, Method, header);

                valueTuple.Succ = result.ProtocolStatusCode == 200 ? true : false;

                var a = result.ResponseData == "" ? "" : DecryptAESHandle(result.ResponseData);
                if (valueTuple.Succ)
                {
                    valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(a);
                }
                else if (result.ProtocolStatusCode == 400 || result.ProtocolStatusCode == 500)
                {
                    var output = JsonConvert.DeserializeObject<ResponseErrorInfo>(a);
                    if (output != null)
                    {
                        error = output.Errors.First().Key.ToString();
                        errCode = string.IsNullOrWhiteSpace(error) ? "ERR913" : ErrorCodeMapping(error);
                        valueTuple.Message = string.Join(",", (((JArray)output.Errors.First().Value).Select(x => (string)x).ToList()));
                    }
                }
                else
                {
                    errCode = "ERR913";
                    valueTuple.Message = result.Message;
                }
                valueTuple.HttpStatusCode = result.ProtocolStatusCode;
            }
            catch (Exception ex)
            {
                errCode = "ERR918";
                valueTuple.Message = ex.Message;
            }


            #region 寫入WebAPILog
            SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
            {
                MKTime = DateTime.Now,
                UPDTime = DateTime.Now,
                WebAPIInput = JsonConvert.SerializeObject(Body),
                WebAPIName = funName,
                WebAPIOutput = JsonConvert.SerializeObject(valueTuple),
                WebAPIURL = requestUrl
            };
            var flag = true;

            List<ErrorInfo> lstError = new List<ErrorInfo>();
            new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref error, ref lstError);
            #endregion


            return valueTuple;
        }


        private (bool Succ, string Message, TResponse Data)
           HotaiMemeberBackendApiPost<TResponse, TRequest>(TRequest Body, string API, ref string errCode, string funName, string access_token = "", string Action = "POST", string Method = "POST")
        {
            (bool Succ, string Message, TResponse Data) valueTuple =
                (false, "", default(TResponse));

            var requestUrl = $"{BackEndURL}{API}";
            string error = "";
            try
            {
                var header = SetBackendRequest(access_token);

                string content = "";

                if (Method == "POST")
                {
                    content = JsonConvert.SerializeObject(Body);
                }

                logger.Info($"Post Body:{content}");

                var result = ApiPost.DoApiPostJson(requestUrl, content, Method, header);

                valueTuple.Succ = result.ProtocolStatusCode == 200 ? true : false;

                if (valueTuple.Succ)
                {
                    valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(result.ResponseData);
                }
                else if (result.ProtocolStatusCode == 400 || result.ProtocolStatusCode == 500)
                {
                    var output = JsonConvert.DeserializeObject<ResponseErrorInfo>(result.ResponseData);
                    if (output != null)
                    {
                        error = output.Errors.First().Key.ToString();
                        errCode = string.IsNullOrWhiteSpace(error) ? "ERR913" : ErrorCodeMapping(error);
                        valueTuple.Message = string.Join(",", (((JArray)output.Errors.First().Value).Select(x => (string)x).ToList()));
                    }
                }
                else
                {
                    errCode = "ERR913";
                    valueTuple.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                errCode = "ERR918";
                valueTuple.Message = ex.Message;
            }


            #region 寫入WebAPILog
            SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
            {
                MKTime = DateTime.Now,
                UPDTime = DateTime.Now,
                WebAPIInput = JsonConvert.SerializeObject(Body),
                WebAPIName = funName,
                WebAPIOutput = JsonConvert.SerializeObject(valueTuple),
                WebAPIURL = requestUrl
            };
            var flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref error, ref lstError);
            #endregion

            return valueTuple;
        }

        private WebHeaderCollection SetRequestHeader(string access_token = "", string authType = "Bearer")
        {
            var header = new WebHeaderCollection();
            if (!string.IsNullOrWhiteSpace(access_token))
            {
                header.Add("Authorization", $"{authType} {access_token}");
            }
            header.Add("APP_ID", AppId);
            header.Add("APP_VERSION", AppVersion);
            header.Add("API_VERSION", ApiVersion);

            return header;
        }

        private WebHeaderCollection SetBackendRequest(string access_token = "", string authType = "Bearer")
        {
            var header = new WebHeaderCollection();
            if (!string.IsNullOrWhiteSpace(access_token))
            {
                header.Add("Authorization", $"{authType} {access_token}");
            }
            header.Add("APP_ID", AppId);
            header.Add("APPKEY", AppKey);

            return header;
        }

        private HotaiMemberRequestBody SetRequestBody(string body, string apiUrl, string action = "POST")
        {
            return new HotaiMemberRequestBody()
            {
                Body = string.IsNullOrEmpty(body) ? "" : EncryptAESHandle(body),
                Method = action,
                Route = apiUrl
            };

        }

        private string EncryptAESHandle(string source)
        {
            string encrypt = "";
            encrypt = AESEncrypt.EncryptAES128(source, Key, IV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

        private string DecryptAESHandle(string encryptData)
        {
            string encrypt = "";
            encrypt = AESEncrypt.DecryptAES128(encryptData, Key, IV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

        /// <summary>
        /// 回傳錯誤代碼Mapping
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string ErrorCodeMapping(string key)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>()
            {
                { "9101","ERR942"},
                { "9102","ERR943"},
                { "9103","ERR944"},
                { "9104","ERR945"},
                { "9105","ERR946"},
                { "9106","ERR947"},
                { "9107","ERR948"},
                { "9108","ERR949"},
                { "9109","ERR950"},
                { "9110","ERR951"},
                { "9111","ERR952"},
                { "9201","ERR953"},
                { "9202","ERR954"},
                { "9203","ERR955"},
                { "9204","ERR956"},
                { "9205","ERR957"},
                { "9301","ERR958"},
                { "9302","ERR959"},
                { "9303","ERR960"},
                { "9304","ERR961"},
                { "9305","ERR962"},
                { "9501","ERR963"},
                { "9502","ERR964"},
                { "9503","ERR965"},
                { "9601","ERR966"},
                { "9602","ERR967"},
                { "AccessToken",    "ERR968"},
                { "Account",        "ERR969"},
                { "Birthday",       "ERR970"},
                { "ConfirmPassword","ERR971"},
                { "Email",          "ERR972"},
                { "Id",             "ERR973"},
                { "MobilePhone",    "ERR974"},
                { "Name",           "ERR975"},
                { "NewPassword",    "ERR976"},
                { "OldPassword",    "ERR977"},
                { "OtpCode",        "ERR978"},
                { "OtpId",          "ERR979"},
                { "Password",       "ERR980"},
                { "RefreshToken",   "ERR981"},
                { "Sex",            "ERR982"},
                { "Type",           "ERR983"},
                { "UseType",        "ERR984"},
                { "Value",          "ERR985"}
            };

            return dics.ContainsKey(key) ? dics.Single(x => x.Key == key).Value : "ERR913";
        }

    }
}
