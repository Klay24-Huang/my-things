using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.OtherService.Taishin;
using Domain.SP.Output;
using Domain.WebAPI.Input.Taishin.Escrow;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Escrow;
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
        private string EscrowBaseURL = ConfigurationManager.AppSettings["EscrowBaseURL"].ToString();//履保URL
        private string AccountBaseURL = ConfigurationManager.AppSettings["TaishinWalletAccountBaseURL"].ToString();
        private string StoreValueCreateAccount = ConfigurationManager.AppSettings["StoreValueCreateAccount"].ToString(); //直接儲值+開戶
        private string StoredMoney = ConfigurationManager.AppSettings["StoredMoney"].ToString(); //直接儲值
        private string TransferStoredValueCreateAccount = ConfigurationManager.AppSettings["TransferStoredValueCreateAccount"].ToString(); //轉贈加開戶
        private string PayTransaction= ConfigurationManager.AppSettings["PayTransaction"].ToString(); //交易扣款
        private string GetAccountValue = ConfigurationManager.AppSettings["GetAccountValue"].ToString(); //查詢帳號明細
        private string GetAccountStatus= ConfigurationManager.AppSettings["GetAccountStatus"].ToString(); //查詢帳號狀態
        private string GetGuaranteeNo = ConfigurationManager.AppSettings["GetGuaranteeNo"].ToString(); //履保/信託序號發行
        private string WriteOffGuaranteeNoJunk = ConfigurationManager.AppSettings["WriteOffGuaranteeNoJunk"].ToString(); //履保/信託序號核銷/報廢
        private string CancelWriteOff = ConfigurationManager.AppSettings["CancelWriteOff"].ToString(); //履保/信託序號取消核銷
        private string ReturnStoreValue = ConfigurationManager.AppSettings["ReturnStoreValue"].ToString();
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

                //20210625 ADD BY ADAM REASON.成功後寫入LOG紀錄
                SPInput_InsStoreValueCreateAccountLog spInput = new SPInput_InsStoreValueCreateAccountLog()
                {
                    GUID = wsInput.GUID,
                    MerchantId = wsInput.MerchantId,
                    AccountId = "",
                    POSId = wsInput.POSId,
                    StoreId = wsInput.StoreId,
                    StoreTransDate = wsInput.StoreTransDate,
                    StoreTransId = wsInput.StoreTransId,
                    TransmittalDate = "",
                    TransDate = output.Result.TransDate,
                    TransId = output.Result.TransId,
                    SourceTransId = wsInput.StoreTransId,
                    TransType = "",
                    AmountType = wsInput.AmountType,
                    Amount = wsInput.Amount,
                    Bonus = wsInput.Bonus,
                    BonusExpiredate = wsInput.BonusExpiredate,
                    BarCode = "",
                    StoreValueReleaseDate = wsInput.StoreValueReleaseDate,
                    StoreValueExpireDate = wsInput.StoreValueExpireDate,
                    SourceFrom = wsInput.SourceFrom,
                    AccountingStatus = "",
                    GiftCardBarCode = wsInput.GiftCardBarCode
                };

                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().InsStoreValueCreateAccountLog(spInput, ref flag, ref errCode, ref lstError);
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
        #region 扣款
        public bool DoPayTransaction(WebAPI_PayTransaction wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WebAPIOutput_PayTransaction output)
        {
            bool flag = true;

            output = DoPayTransactionSend(wsInput, ClientId, utcTimeStamp, SignCode).Result;
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
        private async Task<WebAPIOutput_PayTransaction> DoPayTransactionSend(WebAPI_PayTransaction input, string ClientId, string utcTimeStamp, string SignCode)
        {
            bool flag = false;
            string URL = BaseURL + PayTransaction;
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
            WebAPIOutput_PayTransaction output = null;
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_PayTransaction>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_PayTransaction()
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
                    WebAPIName = "PayTransaction",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + PayTransaction
                };
                flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        #endregion
        #region 轉贈及開戶
        public bool DoTransferStoreValueCreateAccount(WebAPI_TransferStoredValueCreateAccount wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WebAPIOutput_TransferStoreValueCreateAccount output)
        {
            bool flag = true;

            output = DoTransferStoreValueCreateAccountSend(wsInput, ClientId, utcTimeStamp, SignCode).Result;
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
        private async Task<WebAPIOutput_TransferStoreValueCreateAccount> DoTransferStoreValueCreateAccountSend(WebAPI_TransferStoredValueCreateAccount input, string ClientId, string utcTimeStamp, string SignCode)
        {
            bool flag = false;
            string URL = BaseURL + TransferStoredValueCreateAccount;
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
            WebAPIOutput_TransferStoreValueCreateAccount output = null;
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_TransferStoreValueCreateAccount>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_TransferStoreValueCreateAccount()
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
                    WebAPIName = "TransferStoredValueCreateAccount",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + TransferStoredValueCreateAccount
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
        #region 訂閱制相關

        /// 儲值退款
        public bool DoReturnStoreValue(WSInput_ReturnStoreValue wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WSOut_ReturnStoreValue output)
        {
            bool flag = true;
            string url = BaseURL + ReturnStoreValue;
            output = DoGetTaishinApi<WSInput_ReturnStoreValue, WSOut_ReturnStoreValue>(wsInput, ClientId, utcTimeStamp, SignCode, url, "DoReturnStoreValue").Result;
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

        //履保/信託序號發行
        public bool DoGetGuaranteeNo(WSInput_GetGuaranteeNo wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WSOut_GetGuaranteeNo output)
        {
            bool flag = true;
            string url = EscrowBaseURL + GetGuaranteeNo;
            output = DoGetTaishinApi<WSInput_GetGuaranteeNo, WSOut_GetGuaranteeNo>(wsInput, ClientId, utcTimeStamp, SignCode,url, "DoGetGuaranteeNo").Result;
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

        //履保/信託序號核銷/報廢
        public bool DoWriteOffGuaranteeNoJunk(WSInput_WriteOffGuaranteeNoJunk wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WSOut_WriteOffGuaranteeNoJunk output)
        {
            bool flag = true;
            string url = EscrowBaseURL + WriteOffGuaranteeNoJunk;
            output = DoGetTaishinApi<WSInput_WriteOffGuaranteeNoJunk, WSOut_WriteOffGuaranteeNoJunk>(wsInput, ClientId, utcTimeStamp, SignCode, url, "DoWriteOffGuaranteeNoJunk").Result;
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

        //履保/信託序號取消核銷
        public bool DoCancelWriteOff(WSInput_CancelWriteOff wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WSOut_CancelWriteOff output)
        {
            bool flag = true;
            string url = EscrowBaseURL + CancelWriteOff;
            output = DoGetTaishinApi<WSInput_CancelWriteOff, WSOut_CancelWriteOff>(wsInput, ClientId, utcTimeStamp, SignCode, url, "DoCancelWriteOff").Result;
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

        private async Task<T2> DoGetTaishinApi<T1,T2>(T1 input, string ClientId, string utcTimeStamp, string SignCode, string URL,string FunNm) where T2: IWSOut_EscrowBase
        {
            bool flag = false;
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
            var output = Activator.CreateInstance<T2>();
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
                        output = JsonConvert.DeserializeObject<T2>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output.ReturnCode = "9999";
                output.Message = (ex.Message.Length > 200) ? ex.Message.Substring(0, 200) : ex.Message;
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = FunNm,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
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
