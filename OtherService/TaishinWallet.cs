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
using NLog;
using Domain.WebAPI.output.Taishin;
using System.Collections.Specialized;
using System.Web;


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

        private string StoreShopBaseURL = ConfigurationManager.AppSettings["TaishinWalletStoreShopBaseURL"].ToString(); //超商錢包儲值BaseUrl
        private string GetCvsPayToken = ConfigurationManager.AppSettings["GetCvsPayToken"].ToString();          //台新超商APIToken
        private string CreateCvsPayInfo = ConfigurationManager.AppSettings["CreateCvsPayInfo"].ToString();      //超商繳費資訊上傳-新增
        private string GetBarCode = ConfigurationManager.AppSettings["GetBarCode"].ToString();                  //超商條碼查詢
        private string GetBarCodeShortUrl = ConfigurationManager.AppSettings["GetBarCodeShortUrl"].ToString();  //超商條碼短網址查詢
        private string Data = ConfigurationManager.AppSettings["Data"].ToString();             //取台新超商APIToken用
        private string EncKey = ConfigurationManager.AppSettings["EncKey"].ToString();         //取台新超商APIToken用
        private string TaishinCID = ConfigurationManager.AppSettings["TaishinCID"].ToString(); //用戶代碼Base64
        private string HmacKey = ConfigurationManager.AppSettings["HmacKey"].ToString();

        protected static Logger logger = LogManager.GetCurrentClassLogger();


        public string GetHmacVal<T>(T input, string cTxSn)
        {
            string hmacVal = "";
            var jsonString = JsonConvert.SerializeObject(input);
            string formatString = string.Format("{0}{1}", jsonString, cTxSn);
            hmacVal = GenerateHMACVal(formatString, HmacKey);
            return hmacVal;
        }
        public string GenerateHMACVal(string body, string key)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(body);
            using (var hmacSHA256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacSHA256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }

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
            logger.Trace(" DoStoreValueCreateAccountSend : " + JsonConvert.SerializeObject(output));
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
                    AccountId = wsInput.MemberId,
                    POSId = wsInput.POSId,
                    StoreId = wsInput.StoreId,
                    StoreTransDate = wsInput.StoreTransDate,
                    StoreTransId = wsInput.StoreTransId,
                    TransmittalDate = "",
                    TransDate = output.Result.TransDate,
                    TransId = output.Result.TransId,
                    SourceTransId = wsInput.StoreTransId,
                    TransType = "T006",
                    AmountType = wsInput.AmountType,
                    Amount = wsInput.Amount,
                    Bonus = wsInput.Bonus,
                    BonusExpiredate = wsInput.BonusExpiredate,
                    BarCode = "",
                    StoreValueReleaseDate = wsInput.StoreValueReleaseDate,
                    StoreValueExpireDate = wsInput.StoreValueExpireDate,
                    SourceFrom = wsInput.SourceFrom,
                    AccountingStatus = "0",
                    GiftCardBarCode = wsInput.GiftCardBarCode
                };
        
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().InsStoreValueCreateAccountLog(spInput, ref flag, ref errCode, ref lstError);
                logger.Trace(" InsStoreValueCreateAccountLog : " + JsonConvert.SerializeObject(spInput));
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
        
        #region 取台新超商APIToken
        public bool DoGetTaishinCvsPayToken(ref WebAPIOutput_GetTaishinCvsPayToken output)
        {
            bool flag = true;
            output = DoGetTaishinCvsPayTokenSend().Result;
            if (string.IsNullOrWhiteSpace(output.access_token))
            {
                flag = false;
            }
            return flag;
        }

        private async Task<WebAPIOutput_GetTaishinCvsPayToken> DoGetTaishinCvsPayTokenSend()
        {
            bool flag = false;
            string URL = StoreShopBaseURL + GetCvsPayToken;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("Authorization", string.Format("Basic {0}", TaishinCID));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("grant_type", "client_credentials");
            postParams.Add("data", Data);
            postParams.Add("encKey", EncKey);

            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            WebAPIOutput_GetTaishinCvsPayToken output = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());//要發送的字串轉為byte[]

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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetTaishinCvsPayToken>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetTaishinCvsPayToken()
                {
                    ReturnCode = "9999",
                    Message = (ex.Message.Length > 200) ? ex.Message.Substring(0, 200) : ex.Message
                };
            }
            finally
            {
                flag = true;
            }
            return output;
        }
        #endregion

        #region 超商繳費資訊上傳-新增
        public bool DoStoreShopCreateCvsPayInfo(WebAPI_CreateCvsPayInfo wsInput,string accessToken, string hmacVal, ref string errCode, ref WebAPIOutput_CreateCvsPayInfo output)
        {
            bool flag = true;
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            output = DoTaishinWalletStoreShopApiSend<WebAPI_CreateCvsPayInfo, WebAPIOutput_CreateCvsPayInfo>(wsInput, CreateCvsPayInfo, accessToken, hmacVal, funName).Result;

            if (output.header.rtnCode == "ok" && output.body.detail[0].statusCode == "S")
            {
                SPInput_InsWalletStoreShopLog spInput = new SPInput_InsWalletStoreShopLog()
                {
                    CTxSn = wsInput.header.cTxSn,
                    TxSn = output.header.txSN,
                    TxType = wsInput.body.txType,
                    PaymentId = output.body.detail[0].paymentId,
                    CvsType = wsInput.body.cvsCode.cvsType,
                    CvsCode = wsInput.body.cvsCode.cvsCode,
                    PayAmount = wsInput.body.detail[0].payAmount,
                    PayPeriod = wsInput.body.detail[0].payPeriod,
                    DueDate =wsInput.body.detail[0].paidDue,
                    OverPaid = wsInput.body.detail[0].overPaid,
                    CustId =wsInput.body.detail[0].custId,
                    CustMobile = wsInput.body.detail[0].custMobile,
                    CustEmail = wsInput.body.detail[0].custEmail,
                    Memo= wsInput.body.detail[0].memo,          
                    StatusCode = output.body.detail[0].statusCode,
                    StatusDesc = output.body.detail[0].statusDesc
                };
                logger.Trace(" InsWalletStoreShopLog : " + JsonConvert.SerializeObject(spInput));
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().InsWalletStoreShopLog(spInput, ref flag, ref errCode, ref lstError);
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #region 超商繳費條碼查詢
        public bool DoStoreShopGetBarcode(WebAPI_GetBarcode wsInput, string accessToken, string hmacVal, ref string errCode, ref WebAPIOutput_GetBarCode output)
        {
            bool flag = true;
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            output = DoTaishinWalletStoreShopApiSend<WebAPI_GetBarcode, WebAPIOutput_GetBarCode>(wsInput, GetBarCode, accessToken, hmacVal, funName).Result;

            if (output.header.rtnCode == "ok")
            {
                SPInput_UpdWalletStoreShopLog spInput = new SPInput_UpdWalletStoreShopLog()
                {
                    PaymentId = wsInput.body.paymentId,
                    Code1 = output.body.code1,
                    Code2 = output.body.code2,
                    Code3 = output.body.code3,
                    Barcode64 = output.body.barcode64,
                    StatusCode = output.body.statusCode,
                    StatusDesc = output.body.statusDesc
                };
                logger.Trace(" UpdWalletStoreShopLog : " + JsonConvert.SerializeObject(spInput));
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().UpdWalletStoreShopLog(spInput, ref flag, ref errCode, ref lstError);
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 超商繳費條碼短網址查詢
        public bool DoStoreShopGetBarcodeShortUrl(WebAPI_GetBarcodeShortUrl wsInput, string funName, string accessToken, string hmacVal, ref string errCode, ref WebAPIOutput_GetBarCodeShortUrl output)
        {
            bool flag = true;
            output = DoTaishinWalletStoreShopApiSend<WebAPI_GetBarcodeShortUrl, WebAPIOutput_GetBarCodeShortUrl>(wsInput, GetBarCodeShortUrl, accessToken, hmacVal, funName).Result;

            if (output.header.rtnCode == "ok")
            {

            }
            else
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 扣款
        public bool DoPayTransaction(WebAPI_PayTransaction wsInput, string ClientId, string utcTimeStamp, string SignCode, ref string errCode, ref WebAPIOutput_PayTransaction output)
        {
            bool flag = true;

            output = DoPayTransactionSend(wsInput, ClientId, utcTimeStamp, SignCode).Result;
            if (output.ReturnCode == "0000" || output.ReturnCode == "M000")
            {
                //20210630 ADD BY Umeko REASON.扣款成功後寫入LOG紀錄
                SPInput_InsPayTransactionLog spInput = new SPInput_InsPayTransactionLog()
                {
                    GUID = wsInput.GUID,
                    MerchantId = wsInput.MerchantId,
                    AccountId = wsInput.AccountId,
                    BarCode = wsInput.BarCode,
                    POSId = wsInput.POSId,
                    StoreId = wsInput.StoreId,
                    StoreTransDate = wsInput.StoreTransDate,
                    StoreTransId = wsInput.StoreTransId,
                    TransmittalDate = "",
                    TransDate = output.Result.TransDate,
                    TransId = output.Result.TransId,
                    SourceTransId = "", //這參數是退款才需要用到
                    TransType = "T001",
                    BonusFlag = wsInput.BonusFlag,
                    PriceCustody = wsInput.Custody,
                    SmokeLiqueurFlag = wsInput.SmokeLiqueurFlag,
                    Amount = wsInput.Amount,
                    ActualAmount = output.Result.ActualAmount,
                    Bonus = output.Result.Bonus,
                    SourceFrom = wsInput.SourceFrom,
                    AccountingStatus = "0",
                    SmokeAmount= 0,
                    ActualGiftCardAmount =0,
                };
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().InsPayTransactionLog(spInput, ref flag, ref errCode, ref lstError);
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
                //20210702 ADD BY Umeko REASON.扣款成功後寫入LOG紀錄
                SPInput_InsTransferStoreValueLog spInput = new SPInput_InsTransferStoreValueLog()
                {
                    GUID = wsInput.GUID,
                    MerchantId = wsInput.MerchantId,
                    AccountId = wsInput.AccountId,
                    BarCode = wsInput.BarCode,
                    POSId = wsInput.POSId,
                    StoreId = wsInput.StoreId,
                    StoreTransDate = wsInput.StoreTransDate,
                    StoreTransId = wsInput.StoreTransId,
                    TransmittalDate = "",
                    TransDate = output.Result.TransDate,
                    TransId = output.Result.TransId,
                    Amount = wsInput.Amount,
                    ActualAmount = output.Result.ActualAmount,
                    TransAccountId = string.Join("|",wsInput.AccountData.Select(t=>t.TransferAccountId).ToList()),
                    SourceFrom = wsInput.SourceFrom,
                    AmountType = "2",
                };
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new TaishinWalletLog().InsTransferStoreValueCreateAccountLog(spInput, ref flag, ref errCode, ref lstError);
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

        //台新超商交易類通用API
        private async Task<TResponse> DoTaishinWalletStoreShopApiSend<TRequest, TResponse>(TRequest input,string apiUrl, string accessToken,string hmacVal, string funName) where TResponse: WebAPIOutput_Base
        {
            bool flag = false;
            string URL = StoreShopBaseURL + apiUrl;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", accessToken));
            request.Headers.Add("hmacVal", hmacVal);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            var output = Activator.CreateInstance<TResponse>();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                logger.Trace(" postBody : " + postBody);

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
                        output = JsonConvert.DeserializeObject<TResponse>(responseStr);
                        logger.Trace(" responseStr: " + responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output.RtnCode = "9999";
                output.RtnMessage = (ex.Message.Length > 200) ? ex.Message.Substring(0, 200) : ex.Message;
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = funName,
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
