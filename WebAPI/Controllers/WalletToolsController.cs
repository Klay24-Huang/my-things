using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Domain.WebAPI.Input.Taishin.Escrow;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Utils;
using Domain.Flow.Hotai;
using Domain.TB.Hotai;
using NLog;

namespace WebAPI.Controllers
{
    public class WalletToolsController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        
        [HttpPost]
        public Dictionary<string, object> DoWalletTools(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletTools";
            int TSB_WalletBalance = 0;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletTools apiInput = null;
            OAPI_CreditAndWalletQuery apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            HotaipayService HotaipayService = new HotaipayService();
            OFN_HotaiCreditCardList HotaiCards = new OFN_HotaiCreditCardList();
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletTools>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            else
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion

            #region TB
            #region Token判斷
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            //查詢會員狀態
            if (flag && apiInput.FuncName == "AccountStatus")
            {
                TaishinWallet WalletAPI = new TaishinWallet();
                DateTime NowTime = DateTime.UtcNow;
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                int nowCount = 1;
                WebAPI_GetAccountStatus walletStatus = new WebAPI_GetAccountStatus()
                {
                    AccountId = apiInput.AccountID,
                    ApiVersion = "0.1.01",
                    GUID = guid,
                    MerchantId = MerchantId,
                    POSId = "",
                    SourceFrom = "9",
                    StoreId = "",
                    StoreName = ""
                };
                var body = JsonConvert.SerializeObject(walletStatus);
                WebAPIOutput_GetAccountStatus statusOutput = null;
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(MerchantId, utcTimeStamp, body, APIKey);

                flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
                logger.Trace("WalletTools::AccountStatus output=>" + JsonConvert.SerializeObject(statusOutput));
                if (flag)
                {
                    //if (statusOutput.ReturnCode == "0000")
                    //{
                    //    apiOutput.TotalAmount = statusOutput.Result.Amount;
                    //}
                    //if (statusOutput.Result.Status == "2")
                    //{
                    //    apiOutput.HasWallet = 1;
                    //}
                    //TSB_WalletBalance = statusOutput.Result.Amount;
                }
            }

            if (flag && apiInput.FuncName == "AccountValue")
            {
                TaishinWallet WalletAPI = new TaishinWallet();
                DateTime NowTime = DateTime.UtcNow;
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                int nowCount = 1;
                WebAPI_GetAccountValue walletValue = new WebAPI_GetAccountValue()
                {
                    ApiVersion = "0.1.01",
                    GUID = guid,
                    MerchantId = MerchantId,
                    StoreTransId = "",
                    TransId = "",
                    POSId = "",
                    StoreId = "",
                    StoreName = "",
                    AccountId = apiInput.AccountID,
                    SourceFrom = "9",
                    TransType = new List<string> { "T001","T006","T008" },
                    TransStatus = "1",
                    REQStarDate = "",
                    REQEndDate = "",
                    REQStartCount = 1
                };
                var body = JsonConvert.SerializeObject(walletValue);
                WebAPIOutput_GetAccountValue ValueOutput = null;
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(MerchantId, utcTimeStamp, body, APIKey);
                
                flag = WalletAPI.DoGetAccountValue(walletValue, MerchantId, utcTimeStamp, SignCode, ref errCode, ref ValueOutput);
                logger.Trace("WalletTools::AccountValue output=>" + JsonConvert.SerializeObject(ValueOutput));
                if (flag)
                {

                }
            }

            //if (flag && apiInput.FuncName == "ReturnStoreValue")
            //{
            //    TaishinWallet WalletAPI = new TaishinWallet();
            //    DateTime NowTime = DateTime.UtcNow;
            //    string guid = Guid.NewGuid().ToString().Replace("-", "");
            //    int nowCount = 1;
            //    WSInput_ReturnStoreValue walletReturnStoreValue = new WSInput_ReturnStoreValue()
            //    {
            //        ApiVersion = "0.1.01",
            //        GUID = guid,
            //        MerchantId = MerchantId,
            //        POSId = "",
            //        StoreId = "",
            //        StoreName = "",
            //        StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
            //        StoreTransId = "",

            //    };

            //}

            if (flag && apiInput.FuncName == "PayTransaction")
            {
                TaishinWallet WalletAPI = new TaishinWallet();
                DateTime NowTime = DateTime.UtcNow;
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                int nowCount = 1;
                WebAPI_PayTransaction walletPayTrans = new WebAPI_PayTransaction()
                {
                    ApiVersion = "0.1.01",
                    GUID = guid,
                    MerchantId = MerchantId,
                    POSId = "",
                    StoreId = "",
                    StoreName = "",
                    StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                    StoreTransId = string.Format("{0}F{1}", apiInput.USERID, NowTime.ToString("yyMMddHHmmss")),
                    AccountId = apiInput.AccountID,
                    BarCode = "",
                    Amount = apiInput.PayAmount,
                    BonusFlag = "N",
                    Custody = "N",
                    SmokeLiqueurFlag = "N",
                    SourceFrom = "4"
                };

                logger.Trace("WalletTools::PayTransaction input=>" + JsonConvert.SerializeObject(walletPayTrans));
                var body = JsonConvert.SerializeObject(walletPayTrans);
                WebAPIOutput_PayTransaction PayTranOutput = null;
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(MerchantId, utcTimeStamp, body, APIKey);

                flag = WalletAPI.DoPayTransaction(walletPayTrans, MerchantId, utcTimeStamp, SignCode, ref errCode, ref PayTranOutput);
                logger.Trace("WalletTools::PayTransaction output=>" + JsonConvert.SerializeObject(PayTranOutput));
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}