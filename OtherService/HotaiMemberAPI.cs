using Domain.SP.Input.OtherService.Common;
using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.Input.Hotai.Member.Param;
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
        private string FrontEndURL = ConfigurationManager.AppSettings["HotaiMemberFrontEndURL"].ToString();
        private string SingleEntry = ConfigurationManager.AppSettings["HotaiMemberSingleEntry"].ToString();
        private string BackEndURL = ConfigurationManager.AppSettings["HotaiMemberBackEndURL"].ToString();                               
        private string AppId = ConfigurationManager.AppSettings["HotaiAppId"].ToString();
        private string AppVersion = ConfigurationManager.AppSettings["HotaiAppVersion"].ToString();
        private string ApiVersion = ConfigurationManager.AppSettings["HotaiApiVersion"].ToString();
        private string AppKey = ConfigurationManager.AppSettings["HotaiAppKey"].ToString();
        private string Key = ConfigurationManager.AppSettings["HotaiKey"].ToString();
        private string IV = ConfigurationManager.AppSettings["HotaiIV"].ToString();
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
        private string GroupAppsURL = "api/group-apps";                                                    //集團服務
        private string GetMobilePhoneToOneIDURL = "api/subsystem/member/MobilePhoneToOneID";               //使用手機取得會員 OneID
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SendSmsOtpURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_SendSmsOtp>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SmsOtpValidationURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };


            output = DoHotaiMemeberApiSend<WebAPIOutput_OtpValidation>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = RefreshTokenURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Token>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SendEmailOtpURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_SendSmsOtp>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = EmailOtpValidatationURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_OtpValidation>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string action = "GET";
            string apiUrl = CheckTokenURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = token,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody("", apiUrl, action),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = GetEmailURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_GetEmail>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
                flag = true;
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = OtpValidatationURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_OtpValidation>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
                flag = true;
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SignOutURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
        /// <returns></returns>
        public bool DoSignin(WebAPIInput_Signin input, ref WebAPIOutput_Signin output)
        {
            bool flag = false;
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SigninURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Signin>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
                flag = true;
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = CheckSignupURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_CheckSignup>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SignupURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Token>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = SignupProfileURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = ResetPasswordURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string action = "GET";
            string apiUrl = IsMissingMemberProfileURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = token,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody("", apiUrl, action),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_IsMissingMemberProfile>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string action = "GET";
            string apiUrl = GetMemberProfileURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = token,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody("", apiUrl, action),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_GetMemberProfile>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = UpdateMemberProfileURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = ConfirmPasswordURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = UpdateAccountURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = UpdatePasswordURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_Base>(apiRequest).Result;
            if (output.RtnCode == "1000")
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
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = token,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody("", apiUrl, action),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_BenefitsAndPrivacyVersion>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            string json = JsonConvert.SerializeObject(input);
            string encryptString = EncryptAESHandle(json);
            string apiUrl = UpdateBenefitsAndPrivacyVersionURL;
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = json,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody(encryptString, apiUrl),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_BenefitsAndPrivacyVersion>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
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
            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = $"{FrontEndURL}{SingleEntry}",
                ApiUrl = apiUrl,
                DoDecrypt = true,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = token,
                Request = CreateRequestHeader(token),
                Body = CreateRequestBody("", apiUrl, action),
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_GetPrivacy>(apiRequest).Result;
            if (output.RtnCode == "1000")
            {
                flag = true;
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
            string URL = BackEndURL + apiUrl;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Headers.Add("APP_ID", AppId);
            request.Headers.Add("APPKEY", AppKey);
            request.ContentType = "application/json";

            ApiRequestParam apiRequest = new ApiRequestParam()
            {
                BaseUrl = BackEndURL,
                ApiUrl = $"{GetMobilePhoneToOneIDURL}/{mobilePhone}",
                DoDecrypt = false,
                FunName = MethodBase.GetCurrentMethod().Name,
                JsonString = mobilePhone,
                Request = request,
                Body = null,
                SendRequest = false
            };

            output = DoHotaiMemeberApiSend<WebAPIOutput_GetMobilePhoneToOneID>(apiRequest).Result;

            if (output.RtnCode == "1000")
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 會員POST API
        /// </summary>
        /// <typeparam name="String"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="requestBody"></param>
        /// <param name="apiUrl"></param>
        /// <param name="funName"></param>
        /// <returns></returns>               
        private async Task<TResponse> DoHotaiMemeberApiSend<TResponse>(ApiRequestParam apiRequest) where TResponse : WebAPIOutput_Base
        {
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            string URL = apiRequest.BaseUrl + apiRequest.ApiUrl;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var postBody = JsonConvert.SerializeObject(apiRequest.Body);
            var output = Activator.CreateInstance<TResponse>();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                if (apiRequest.SendRequest)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]
                    using (Stream reqStream = apiRequest.Request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                #region 發出Request
                using (WebResponse response = apiRequest.Request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        string responseStr = "";
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        if (!string.IsNullOrWhiteSpace(responseStr))
                        {
                            var json = apiRequest.DoDecrypt ? DecryptAESHandle(responseStr) : responseStr;
                            output = JsonConvert.DeserializeObject<TResponse>(json);
                        }
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
                            var json = apiRequest.DoDecrypt ? DecryptAESHandle(responseStr) : responseStr;
                            output = JsonConvert.DeserializeObject<TResponse>(json);
                            info.ErrorCode = output.errors.First().Key.ToString();
                            info.ErrorMsg = string.Join(",", (((JArray)output.errors.First().Value).Select(x => (string)x).ToList()));
                        }
                    }

                    output.RtnCode = info.ErrorCode;
                    output.RtnMessage = info.ErrorMsg;
                }
                webResponse.Close();
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
                    WebAPIInput = JsonConvert.SerializeObject(apiRequest.JsonString),
                    WebAPIName = apiRequest.FunName,
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
            string URL = $"{FrontEndURL}{SingleEntry}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Authorization", $"{authType} {token}");
            }
            request.Headers.Add("APP_ID",AppId);
            request.Headers.Add("APP_VERSION", AppVersion);
            request.Headers.Add("API_VERSION", ApiVersion);
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
            encrypt = AESEncrypt.EncryptAES128(source, Key, IV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

        private string DecryptAESHandle(string encryptData)
        {
            string encrypt = "";
            encrypt = AESEncrypt.DecryptAES128(encryptData, Key, IV, CipherMode.CBC, PaddingMode.PKCS7);
            return encrypt;
        }

    }
}
