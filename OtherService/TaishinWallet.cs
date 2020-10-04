﻿using Domain.SP.Input.OtherService.Common;
using Domain.SP.Output;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 台新錢包
    /// </summary>
    public class TaishinWallet
    {
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();
        private string AccountBaseURL = ConfigurationManager.AppSettings["TaishinWalletAccountBaseURL"].ToString();
        private string StoreValueCreateAccount = ConfigurationManager.AppSettings["StoreValueCreateAccount"].ToString(); //直接儲值+開戶
        private string StoredMoney = ConfigurationManager.AppSettings["StoredMoney"].ToString(); //直接儲值
        private string TransferStoredValueCreateAccount = ConfigurationManager.AppSettings["TransferStoredValueCreateAccount"].ToString(); //轉贈加開戶
        private string GetAccountValue = ConfigurationManager.AppSettings["GetAccountValue"].ToString(); //查詢帳號明細
        private string GetAccountStatus= ConfigurationManager.AppSettings["GetAccountStatus"].ToString(); //查詢帳號狀態
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public string GenerateSignCode(string ClientId,string utcTimeStamp,string body,string apiKey)
        {
            string SignCode = "";
            
            string tmpSource = string.Format("{0}&{1}&{2}&{3}", ClientId, utcTimeStamp, body, apiKey);
            byte[] bytes = Encoding.GetEncoding(950).GetBytes(tmpSource);
            SignCode = ComputeSha256Hash(bytes).ToUpper();
            return SignCode;
        }
        #region 直接儲值+開戶
        public bool DoStoreValueCreateAccount(WebAPI_CreateAccountAndStoredMoney wsInput,string ClientId,string utcTimeStamp,string SignCode, ref string errCode, ref WebAPIOutput_StoreValueCreateAccount output)
        {
            bool flag = true;

            output = DoStoreValueCreateAccountSend(wsInput, ClientId,utcTimeStamp,SignCode).Result;
            if (output.ReturnCode == "0000" || output.ReturnCode=="M000" )
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        private async Task<WebAPIOutput_StoreValueCreateAccount> DoStoreValueCreateAccountSend(WebAPI_CreateAccountAndStoredMoney input,string ClientId,string utcTimeStamp,string SignCode)
        {
            bool flag = false;
            string URL = BaseURL + StoreValueCreateAccount;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", APIToken));
            request.Headers.Add("ClientId", ClientId);
            request.Headers.Add("UtcTimeStamp", utcTimeStamp);
            request.Headers.Add("SignCode", SignCode);
            request.Method = "POST";
            request.ContentType = "application/json";
            
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            WebAPIOutput_StoreValueCreateAccount output=null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_StoreValueCreateAccount>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_StoreValueCreateAccount()
                {
                    ReturnCode = "9999",
                    Message =(ex.Message.Length>200)? ex.Message.Substring(0, 200):ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "StoreValueCreateAccount",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + StoreValueCreateAccount
                };
                 flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        #endregion
        #region 查詢帳號狀態
        public bool DoGetAccountStatus(WebAPI_GetAccountStatus wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WebAPIOutput_GetAccountStatus output)
        {
            bool flag = true;

            output = DoGetAccountStatusSend(wsInput, ClientId, utcTimeStamp, SignCode).Result;
            if (output.ReturnCode == "0000" || output.ReturnCode == "M000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        private async Task<WebAPIOutput_GetAccountStatus> DoGetAccountStatusSend(WebAPI_GetAccountStatus input, string ClientId, string utcTimeStamp, string SignCode)
        {
            bool flag = false;
            string URL = AccountBaseURL + GetAccountStatus;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", APIToken));
            request.Headers.Add("ClientId", ClientId);
            request.Headers.Add("UtcTimeStamp", utcTimeStamp);
            request.Headers.Add("SignCode", SignCode);
            request.Method = "POST";
            request.ContentType = "application/json";

            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            WebAPIOutput_GetAccountStatus output = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetAccountStatus>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetAccountStatus()
                {
                    ReturnCode = "9999",
                    Message = (ex.Message.Length > 200) ? ex.Message.Substring(0, 200) : ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetAccountStatus",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = AccountBaseURL + GetAccountStatus
                };
                flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        #endregion
        #region 查詢帳號明細
        public bool DoGetAccountValue(WebAPI_GetAccountValue wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WebAPIOutput_GetAccountValue output)
        {
            bool flag = true;

            output = DoGetAccountValueSend(wsInput, ClientId, utcTimeStamp, SignCode).Result;
            if (output.ReturnCode == "0000" || output.ReturnCode == "M000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        private async Task<WebAPIOutput_GetAccountValue> DoGetAccountValueSend(WebAPI_GetAccountValue input, string ClientId, string utcTimeStamp, string SignCode)
        {
            bool flag = false;
            string URL = AccountBaseURL + GetAccountValue;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", APIToken));
            request.Headers.Add("ClientId", ClientId);
            request.Headers.Add("UtcTimeStamp", utcTimeStamp);
            request.Headers.Add("SignCode", SignCode);
            request.Method = "POST";
            request.ContentType = "application/json";

            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            WebAPIOutput_GetAccountValue output = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetAccountValue>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetAccountValue()
                {
                    ReturnCode = "9999",
                    Message = (ex.Message.Length > 200) ? ex.Message.Substring(0, 200) : ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetAccountValue",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = AccountBaseURL + GetAccountValue
                };
                flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        #endregion
        private string ByteToString(byte[] source)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                builder.Append(source[i].ToString("x2"));
            }
            return builder.ToString().ToUpper();
        }
        private string ComputeSha256Hash(byte[] SourceBytes)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(SourceBytes);

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}