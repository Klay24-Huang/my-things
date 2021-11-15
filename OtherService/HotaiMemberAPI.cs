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
        private string HotaiMemberFrontEndURL = ConfigurationManager.AppSettings["HotaiMemberFrontEndURL"].ToString();                                //FrontEndApi
        private string HotaiMemberBackEndURL = ConfigurationManager.AppSettings["HotaiMemberBackEndURL"].ToString();                                  //BackEndApi
        private string CheckSignupURL = ConfigurationManager.AppSettings["CheckSignupURL"].ToString();                                                //註冊檢查  
        private string SendSmsOtpURL = ConfigurationManager.AppSettings["SendSmsOtpURL"].ToString();                                                  //發送簡訊OTP
        private string SmsOtpValidationURL = ConfigurationManager.AppSettings["SmsOtpValidationURL"].ToString();                                      //簡訊 OTP 驗證
        private string RefreshTokenURL = ConfigurationManager.AppSettings["RefreshTokenURL"].ToString();                                              //更新 Token
        private string SendEmailOtpURL = ConfigurationManager.AppSettings["SendEmailOtpURL"].ToString();                                              //發送 Email OTP
        private string EmailOtpValidatationURL = ConfigurationManager.AppSettings["EmailOtpValidatationURL"].ToString();                              //Email OTP 驗證
        private string CheckTokenURL = ConfigurationManager.AppSettings["CheckTokenURL"].ToString();                                                  //檢查 Token
        private string GetEmailURL = ConfigurationManager.AppSettings["GetEmailURL"].ToString();                                                      //取得 Email
        private string OtpValidatationURL = ConfigurationManager.AppSettings["OtpValidatationURL"].ToString();                                        //驗證會員資訊,取得 OTP 編號
        private string SignOutURL = ConfigurationManager.AppSettings["SignOutURL"].ToString();                                                        //登出
        private string SigninURL = ConfigurationManager.AppSettings["SigninURL"].ToString();                                                          //登入
        private string SignupURL = ConfigurationManager.AppSettings["SignupURL"].ToString();                                                          //註冊
        private string SignupProfileURL = ConfigurationManager.AppSettings["SignupProfileURL"].ToString();                                            //註冊個人資料(一般)
        private string ResetPasswordURL = ConfigurationManager.AppSettings["ResetPasswordURL"].ToString();                                            //重設密碼
        private string IsMissingMemberProfileURL = ConfigurationManager.AppSettings["IsMissingMemberProfileURL"].ToString();                          //缺少個人資料
        private string GetMemberProfileURL = ConfigurationManager.AppSettings["GetMemberProfileURL"].ToString();                                      //取得個人資料
        private string UpdateMemberProfileURL = ConfigurationManager.AppSettings["UpdateMemberProfileURL"].ToString();                                //更新個人資料
        private string ConfirmPasswordURL = ConfigurationManager.AppSettings["ConfirmPasswordURL"].ToString();                                        //確認密碼
        private string UpdateAccountURL = ConfigurationManager.AppSettings["UpdateAccountURL"].ToString();                                            //修改帳號
        private string UpdatePasswordURL = ConfigurationManager.AppSettings["UpdatePasswordURL"].ToString();                                          //修改密碼
        private string CheckBenefitsAndPrivacyVersionURL = ConfigurationManager.AppSettings["CheckBenefitsAndPrivacyVersionURL"].ToString();          //檢查會員權益及隱私條款版本
        private string UpdateBenefitsAndPrivacyVersionURL = ConfigurationManager.AppSettings["UpdateBenefitsAndPrivacyVersionURL"].ToString();        //同意新款會員權益及隱私條款
        private string GetPrivacyURL = ConfigurationManager.AppSettings["GetPrivacyURL"].ToString();                                                  //取得會員權益及隱私條款
        private string GroupAppsURL = ConfigurationManager.AppSettings["GroupAppsURL"].ToString();                                                    //集團服務
        private string GetMobilePhoneToOneIDURL = ConfigurationManager.AppSettings["GetMobilePhoneToOneIDURL"].ToString();                            //使用手機取得會員 OneID
        private string HotaiAppId = ConfigurationManager.AppSettings["HotaiAppId"].ToString();
        private string HotaiAppVersion = ConfigurationManager.AppSettings["HotaiAppVersion"].ToString();
        private string HotaiApiVersion = ConfigurationManager.AppSettings["HotaiApiVersion"].ToString();
        private string HotaiAppKey = ConfigurationManager.AppSettings["HotaiAppKey"].ToString();
        private string HotaiKey = ConfigurationManager.AppSettings["HotaiKey"].ToString();
        private string HotaiIV = ConfigurationManager.AppSettings["HotaiIV"].ToString();
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 發送簡訊 OTP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSendSmsOtp(WebAPIInput_SendSmsOtp input, ref WebAPIOutput_SendSmsOtp output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SendSmsOtpURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_SendSmsOtp>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.otpCode = wsOut.otpCode;
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 簡訊 OTP 驗證
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSmsOtpValidation(WebAPIInput_SmsOtpValidation input, ref WebAPIOutput_OtpValidation output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SmsOtpValidationURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_OtpValidation>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.otpId = wsOut.otpId;
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 更新 Token
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoRefreshToken(WebAPIInput_RefreshToken input, ref WebAPIOutput_Token output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = RefreshTokenURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Token>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.access_token = wsOut.access_token;
                output.refresh_token = wsOut.refresh_token;
                output.token_type = wsOut.token_type;
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 發送 Email OTP
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSendEmailOtp(string token, WebAPIInput_SendEmailOtp input, ref WebAPIOutput_SendSmsOtp output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SendEmailOtpURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_SendSmsOtp>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.otpCode = wsOut.otpCode;
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// Email OTP 驗證
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoEmailOtpValidatation(string token, WebAPIInput_EmailOtpValidatation input, ref WebAPIOutput_OtpValidation output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = EmailOtpValidatationURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_OtpValidation>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.otpId = wsOut.otpId;
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 檢查 Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCheckToken(string token, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string apiUrl = CheckTokenURL;
            string action = "GET";
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <returns></returns>
        public bool DoGetEmail(WebAPIInput_GetEmail input, ref WebAPIOutput_GetEmail output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = GetEmailURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_GetEmail>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.email = wsOut.email;
                flag = true;
            }
            else
            {
                output.RtnMessage = wsOut.RtnMessage;
            }
            return flag;

        }

        /// <summary>
        /// 驗證會員資訊，取得 OTP 編號
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoOtpValidatation(WebAPIInput_OtpValidatation input, ref WebAPIOutput_OtpValidation output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = OtpValidatationURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_OtpValidation>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.otpId = wsOut.otpId;
                flag = true;
            }
            else
            {
                output.RtnMessage = wsOut.RtnMessage;
            }
            return flag;

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSignOut(string token, WebAPIInput_SignOut input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SignOutURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                flag = true;
            }
            else
            {
                output.RtnMessage = wsOut.RtnMessage;
            }
            return flag;

        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSignin(WebAPIInput_Signin input, ref WebAPIOutput_Signin output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SigninURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Signin>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.isCheckEmail = wsOut.isCheckEmail;
                output.memberState = wsOut.memberState;
                output.access_token = wsOut.access_token;
                output.refresh_token = wsOut.refresh_token;
                output.token_type = wsOut.token_type;
                flag = true;
            }
            else
            {
                output.RtnMessage = wsOut.RtnMessage;
            }
            return flag;
        }

        /// <summary>
        /// 註冊檢查
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns> 
        public bool DoCheckSignup(WebAPIInput_CheckSignup input, ref WebAPIOutput_CheckSignup output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = CheckSignupURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_CheckSignup>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.isSignup = wsOut.isSignup;
                output.status = wsOut.status;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSignup(WebAPIInput_Signup input, ref WebAPIOutput_Token output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SignupURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Token>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.access_token = wsOut.access_token;
                output.refresh_token = wsOut.refresh_token;
                output.token_type = wsOut.token_type;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 註冊個人資料(一般)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoSignupProfile(string token, WebAPIInput_SignupProfile input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = SignupProfileURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoResetPassword(WebAPIInput_ResetPassword input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = ResetPasswordURL;
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <returns></returns>
        public bool DoIsMissingMemberProfile(string token, ref WebAPIOutput_IsMissingMemberProfile output)
        {
            bool flag = false;
            string apiUrl = IsMissingMemberProfileURL;
            string action = "GET";
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_IsMissingMemberProfile>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.isMissing = wsOut.isMissing;
                output.memberState = wsOut.memberState;
                output.missingBirthday = wsOut.missingBirthday;
                output.missingId = wsOut.missingId;
                output.missingName = wsOut.missingName;
                output.missingEmail = wsOut.missingEmail;
                output.missingSex = wsOut.missingSex;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 取得個人資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGetMemberProfile(string token, ref WebAPIOutput_GetMemberProfile output)
        {
            bool flag = false;
            string apiUrl = GetMemberProfileURL;
            string action = "GET";
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_GetMemberProfile>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.name = wsOut.name;
                output.email = wsOut.email;
                output.birthday = wsOut.birthday;
                output.sex = wsOut.sex;
                output.id = wsOut.id;
                output.county = wsOut.county;
                output.township = wsOut.township;
                output.address = wsOut.address;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoUpdateMemberProfile(string token, WebAPIInput_UpdateMemberProfile input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = UpdateMemberProfileURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoConfirmPassword(string token, WebAPIInput_ConfirmPassword input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = ConfirmPasswordURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoUpdateAccount(string token, WebAPIInput_UpdateAccount input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = UpdateAccountURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoUpdatePassword(string token, WebAPIInput_UpdatePassword input, ref WebAPIOutput_Base output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = UpdatePasswordURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_Base>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
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
        /// <returns></returns>
        public bool DoCheckBenefitsAndPrivacyVersion(string token, ref WebAPIOutput_BenefitsAndPrivacyVersion output)
        {
            bool flag = false;
            string action = "GET";
            string apiUrl = CheckBenefitsAndPrivacyVersionURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_BenefitsAndPrivacyVersion>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.memberBenefitsVersion = wsOut.memberBenefitsVersion;
                output.memberBenefits = wsOut.memberBenefits;
                output.privacyPolicyVersion = wsOut.privacyPolicyVersion;
                output.privacyPolicy = wsOut.privacyPolicy;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 同意新款會員權益及隱私條款
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoUpdateBenefitsAndPrivacyVersion(string token, WebAPIInput_UpdateBenefitsAndPrivacyVersion input, ref WebAPIOutput_BenefitsAndPrivacyVersion output)
        {
            bool flag = false;
            string encryptString = EncryptAESHandle(JsonConvert.SerializeObject(input));
            string apiUrl = UpdateBenefitsAndPrivacyVersionURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody(encryptString, apiUrl);
            var wsOut = DoHotaiMemeberApiSend<WebAPIInput_UpdateBenefitsAndPrivacyVersion, WebAPIOutput_BenefitsAndPrivacyVersion>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.memberBenefitsVersion = wsOut.memberBenefitsVersion;
                output.memberBenefits = wsOut.memberBenefits;
                output.privacyPolicyVersion = wsOut.privacyPolicyVersion;
                output.privacyPolicy = wsOut.privacyPolicy;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 取得會員權益及隱私條款
        /// </summary>
        /// <param name="token"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGetPrivacy(string token, ref WebAPIOutput_GetPrivacy output)
        {
            bool flag = false;
            string action = "GET";
            string apiUrl = GetPrivacyURL;
            HttpWebRequest request = CreateRequestHeader(token);
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            var wsOut = DoHotaiMemeberApiSend<string, WebAPIOutput_GetPrivacy>(request, requestBody, apiUrl, MethodBase.GetCurrentMethod().Name).Result;
            if (wsOut.RtnCode == "1000")
            {
                output.memberBenefits = wsOut.memberBenefits;
                output.privacyPolicy = wsOut.privacyPolicy;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 集團服務
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGroupApps(string device, ref WebAPIOutput_GroupApps output)
        {
            bool flag = false;
            string action = "GET";
            string funName = MethodBase.GetCurrentMethod().Name;
            string apiUrl = $"{GroupAppsURL}/deviceOS?{device}";
            HttpWebRequest request = CreateRequestHeader();
            RequestBody requestBody = CreateRequestBody("", apiUrl, action);
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    string postBody = JsonConvert.SerializeObject(requestBody);//將匿名物件序列化為json字串
                    byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                    }

                    string responseStr = "";
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = reader.ReadToEnd();
                            RTime = DateTime.Now;
                            if (!string.IsNullOrWhiteSpace(responseStr))
                            {
                                var json = DecryptAESHandle(responseStr);
                                logger.Trace($"{funName} json: " + json);
                                IEnumerable<GroupApps> result = JsonConvert.DeserializeObject<IEnumerable<GroupApps>>(json);
                                output.groupApps = (List<GroupApps>)result;
                            }
                            output.RtnCode = "1000";
                        }
                    }
                    flag = output.RtnCode == "1000" ? true : false;
                }
                catch (WebException ex)
                {
                    WebResponse webResponse = (WebResponse)ex.Response;
                    var info = new ErrorInfo();
                    if (webResponse != null)
                    {
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            string responseStr = reader.ReadToEnd();
                            if (!string.IsNullOrWhiteSpace(responseStr))
                            {
                                var json = DecryptAESHandle(responseStr);
                                var result = JsonConvert.DeserializeObject<WebAPIOutput_Base>(json);
                                logger.Trace($"{funName} json: " + json);
                                info.ErrorCode = result.errors.First().Key.ToString();
                                info.ErrorMsg = string.Join(",", (((JArray)result.errors.First().Value).Select(x => (string)x).ToList()));
                            }
                        }
                    }
                    webResponse.Close();
                    RTime = DateTime.Now;
                    output.RtnCode = info.ErrorCode;
                    output.RtnMessage = info.ErrorMsg;
                    logger.Trace($"{funName} Exception: " + ex.Message);
                }
            }
            catch (Exception e)
            {
                output.RtnCode = "9999";
                output.RtnMessage = e.Message;
            }

            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = "",
                    WebAPIName = funName,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = apiUrl
                };
                flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return flag;
        }

        /// <summary>
        /// 使用手機取得會員
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGetMobilePhoneToOneID(string mobilePhone, ref WebAPIOutput_GetMobilePhoneToOneID output)
        {
            bool flag = false;
            string apiUrl = $"{GetMobilePhoneToOneIDURL}/{mobilePhone}";
            string URL = HotaiMemberBackEndURL + apiUrl;
            string funName = MethodBase.GetCurrentMethod().Name;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Headers.Add("APP_ID", HotaiAppId);
            request.Headers.Add("APPKEY", HotaiAppKey);
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    #region 發出Request
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            string responseStr = "";
                            responseStr = reader.ReadToEnd();
                            RTime = DateTime.Now;
                            output = JsonConvert.DeserializeObject<WebAPIOutput_GetMobilePhoneToOneID>(responseStr);
                            output.RtnCode = "1000";
                        }
                    }
                    #endregion
                }
                catch (WebException ex)
                {
                    WebResponse webResponse = (WebResponse)ex.Response;
                    var info = new ErrorInfo();
                    if (webResponse != null)
                    {
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            string responseStr = reader.ReadToEnd();
                            if (!string.IsNullOrWhiteSpace(responseStr))
                            {
                                output = JsonConvert.DeserializeObject<WebAPIOutput_GetMobilePhoneToOneID>(responseStr);
                                //logger.Trace($"{funName} json: " + json);
                                info.ErrorCode = output.errors.First().Key.ToString();
                                info.ErrorMsg = string.Join(",", (((JArray)output.errors.First().Value).Select(x => (string)x).ToList()));
                            }
                        }
                    }
                    webResponse.Close();

                    RTime = DateTime.Now;
                    output.RtnCode = info.ErrorCode;
                    output.RtnMessage = info.ErrorMsg;
                }

                flag = output.RtnCode == "1000" ? true : false;
            }
            catch (Exception e)
            {
                output.RtnCode = "9999";
                output.RtnMessage = e.Message;
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(mobilePhone),
                    WebAPIName = funName,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
                };

                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return flag;
        }

        /// <summary>
        /// 會員通用API
        /// </summary>
        /// <typeparam name="String"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="requestBody"></param>
        /// <param name="apiUrl"></param>
        /// <param name="funName"></param>
        /// <returns></returns>               
        private async Task<TResponse> DoHotaiMemeberApiSend<String, TResponse>(HttpWebRequest request, RequestBody requestBody, string apiUrl, string funName) where TResponse : WebAPIOutput_Base
        {
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            string URL = HotaiMemberBackEndURL + apiUrl;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var postBody = JsonConvert.SerializeObject(requestBody);
            var output = Activator.CreateInstance<TResponse>();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                    }

                    #region 發出Request
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                    if (myHttpWebResponse.StatusCode== HttpStatusCode.OK)
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                            {
                                string responseStr = "";
                                responseStr = reader.ReadToEnd();
                                RTime = DateTime.Now;
                                if (!string.IsNullOrWhiteSpace(responseStr))
                                {
                                    var json = DecryptAESHandle(responseStr);
                                    logger.Trace($"{funName} json: " + json);
                                    output = JsonConvert.DeserializeObject<TResponse>(json);
                                }
                                output.RtnCode = "1000";
                            }
                        }
                    }
                    else
                    {
                        //using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        //{
                        //    string responseStr = reader.ReadToEnd();
                        //    if (!string.IsNullOrWhiteSpace(responseStr))
                        //    {
                        //        var json = DecryptAESHandle(responseStr);
                        //        output = JsonConvert.DeserializeObject<TResponse>(json);
                        //        logger.Trace($"{funName} json: " + json);
                        //        info.ErrorCode = output.errors.First().Key.ToString();
                        //        info.ErrorMsg = string.Join(",", (((JArray)output.errors.First().Value).Select(x => (string)x).ToList()));
                        //    }
                        //}
                    }
                    #endregion

                }
                catch (WebException ex)
                {
                    WebResponse webResponse = (WebResponse)ex.Response;
                    var info = new ErrorInfo();
                    if (webResponse != null)
                    {
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            string responseStr = reader.ReadToEnd();
                            if (!string.IsNullOrWhiteSpace(responseStr))
                            {
                                var json = DecryptAESHandle(responseStr);
                                output = JsonConvert.DeserializeObject<TResponse>(json);
                                logger.Trace($"{funName} json: " + json);
                                info.ErrorCode = output.errors.First().Key.ToString();
                                info.ErrorMsg = string.Join(",", (((JArray)output.errors.First().Value).Select(x => (string)x).ToList()));
                            }
                        }
                    }
                    webResponse.Close();
                }

            }
            catch (Exception e)
            {
                output.RtnCode = "9999";
                output.RtnMessage = e.Message;
                //logger.Trace($"{funName} Exception: " + ex.Message);
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(postBody),
                    WebAPIName = funName,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
                };

                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        private HttpWebRequest CreateRequestHeader(string token = "", string action = "POST", string authType = "Bearer")
        {
            string URL = HotaiMemberFrontEndURL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Authorization", $"{authType} {token}");
            }
            request.Headers.Add("APP_ID", HotaiAppId);
            request.Headers.Add("APP_VERSION", HotaiAppVersion);
            request.Headers.Add("API_VERSION", HotaiApiVersion);
            request.Method = action;
            request.ContentType = "application/json";
            return request;
        }

        private RequestBody CreateRequestBody(string body, string apiUrl, string action = "POST")
        {
            RequestBody requestBody = new RequestBody()
            {
                Body = body,
                Method = action,
                Route = apiUrl
            };

            return requestBody;
        }

        private string EncryptAESHandle(string source)
        {
            string encrypt = "";
            encrypt = AESEncrypt.EncryptAES128(source, HotaiKey, HotaiIV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

        private string DecryptAESHandle(string encryptData)
        {
            string encrypt = "";
            encrypt = AESEncrypt.DecryptAES128(encryptData, HotaiKey, HotaiIV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

        public class RequestBody
        {
            public string Body { get; set; }
            public string Method { get; set; }
            public string Route { get; set; }

        }
    }
}
