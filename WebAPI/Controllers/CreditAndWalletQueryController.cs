using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
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

    /// <summary>
    /// 查詢綁卡及錢包
    /// </summary>
    public class CreditAndWalletQueryController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoCreditAndWalletQuery(Dictionary<string, object> value)
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
            string funName = "CreditAndWalletQueryController";
            int TSB_WalletBalance = 0;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CreditAndWalletQuery apiInput = null;
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
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);

            if (flag)
            {
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
            #region 信用卡
            if (flag)
            {
                //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);
                if (ds.Tables.Count > 0)
                {
                    apiOutput = new OAPI_CreditAndWalletQuery()
                    {
                        HasBind = (ds.Tables[0].Rows.Count == 0) ? 0 : 1,
                        BindListObj = new List<Models.Param.Output.PartOfParam.CreditCardBindList>()
                    };
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Models.Param.Output.PartOfParam.CreditCardBindList obj = new Models.Param.Output.PartOfParam.CreditCardBindList()
                        {
                            AvailableAmount = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["AvailableAmount"].ToString()),
                            BankNo = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["BankNo"].ToString()),
                            CardName = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardName"].ToString()),
                            CardNumber = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardNumber"].ToString()),
                            CardToken = baseVerify.BaseCheckString(ds.Tables[0].Rows[i]["CardToken"].ToString())
                        };
                        apiOutput.BindListObj.Add(obj);
                    }
                }
                ds.Dispose();
            }
            #endregion
            #region 台新錢包(MARK)
            //// 錢包先點掉，防火牆不通，先不取資料
            //if (flag)
            //{
            //    #region 取個人資料
            //    string SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfo);
            //    SPInput_GetWalletInfo SPInput = new SPInput_GetWalletInfo()
            //    {
            //        IDNO = IDNO,
            //        LogID = LogID,
            //        Token = Access_Token
            //    };
            //    SPOutput_GetWalletInfo SPOutput = new SPOutput_GetWalletInfo();
            //    SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo> sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
            //    flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
            //    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
            //    #endregion
            //    TaishinWallet WalletAPI = new TaishinWallet();
            //    DateTime NowTime = DateTime.UtcNow;
            //    string guid = Guid.NewGuid().ToString().Replace("-", "");
            //    int nowCount = 1;
            //    WebAPI_GetAccountStatus walletStatus = new WebAPI_GetAccountStatus()
            //    {
            //        AccountId = SPOutput.WalletAccountID,
            //        ApiVersion = "0.1.01",
            //        GUID = guid,
            //        MerchantId = MerchantId,
            //        POSId = "",
            //        SourceFrom = "9",
            //        StoreId = "",
            //        StoreName = ""
            //    };
            //    var body = JsonConvert.SerializeObject(walletStatus);
            //    WebAPIOutput_GetAccountStatus statusOutput = null;
            //    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            //    string SignCode = WalletAPI.GenerateSignCode(MerchantId, utcTimeStamp, body, APIKey);

            //    flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
            //    if (flag)
            //    {
            //        if (statusOutput.ReturnCode == "0000")
            //        {
            //            apiOutput.TotalAmount = statusOutput.Result.Amount;
            //        }
            //        if (statusOutput.Result.Status == "2")
            //        {
            //            apiOutput.HasWallet = 1;
            //        }
            //        TSB_WalletBalance = statusOutput.Result.Amount;
            //    }
            //}
            #endregion
            #region 和泰PAY
            if (flag)
            {
                var objQueryCards = new IFN_QueryCardList
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    PRGName = funName,
                    insUser = IDNO
                };
                string hotaiErrorCode = "";
                var HotaiFlag = HotaipayService.DoQueryCardList(objQueryCards, ref HotaiCards, ref hotaiErrorCode);

                if (HotaiFlag)
                {
                    if (HotaiCards.CreditCards != null && HotaiCards.CreditCards.Count > 0)
                    {
                        var DefaultCard = HotaiCards.CreditCards.Find(x => x.IsDefault == 1);

                        apiOutput.HasHotaiPay = (DefaultCard != null) ? 1 : 0;
                        apiOutput.HotaiListObj = HotaiCards.CreditCards;
                    }
                    else
                    {
                        apiOutput.HasHotaiPay = 0;
                        apiOutput.HotaiListObj = new List<HotaiCardInfo>();
                    }
                }
                else
                {
                    apiOutput.HasHotaiPay = 0;
                    apiOutput.HotaiListObj = new List<HotaiCardInfo>();
                }
            }
            #endregion
            #region 回傳結果
            if (flag)
            {
                string SPName = "usp_CreditAndWalletQuery_Q01";
                SPInput_CreditAndWalletQuery spInput = new SPInput_CreditAndWalletQuery
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOut_CreditAndWalletQuery spOut = new SPOut_CreditAndWalletQuery();
                SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery> sqlHelp = new SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    apiOutput.PayMode = spOut.PayMode;
                    apiOutput.HasWallet = spOut.WalletStatus == "2" ? 1 : 0;
                    apiOutput.TotalAmount = spOut.WalletAmout; 
                    //apiOutput.TotalAmount = TSB_WalletBalance;//20211221 UPD BY YANKEY REASON:改抓台新紀錄的餘額
                    apiOutput.MEMSENDCD = spOut.MEMSENDCD;
                    apiOutput.UNIMNO = spOut.UNIMNO;
                    apiOutput.CARRIERID = spOut.CARRIERID;
                    apiOutput.NPOBAN = spOut.NPOBAN;
                    apiOutput.AutoStored = spOut.AutoStored;
                    //logger.Trace($"\n\tIDNO = {IDNO} \n\t HasWallet = {apiOutput.HasWallet} \n\t TSB_WalletBalance = {TSB_WalletBalance}\n\t spOut.WalletAmout ={spOut.WalletAmout}");
                }
            }
            #endregion
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