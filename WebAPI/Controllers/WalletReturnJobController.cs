using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Wallet;
using Domain.TB;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包交易退款排程
    /// </summary>
    public class WalletReturnJobController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private readonly string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private readonly string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private readonly string ApiVersion = ConfigurationManager.AppSettings["TaishinWalletApiVersion"].ToString();

        private CommonFunc BaseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoWalletReturnJob()
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletReturnJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Token token = null;
            NullOutput apiOutput = new NullOutput();
            BaseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<SPOutput_GetWalletReturn> returnList = null;
            WebAPIOutput_Refund output = null;
            TaishinWallet taishinWallet = new TaishinWallet();
            string exMsg = "";
            string dbError = "";

            #endregion
            #region 寫入API Log
            string ClientIP = BaseVerify.GetClientIp(Request);
            bool flag = BaseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);
            #endregion
            #region TB
            if (flag)
            {
                //取得退款清單
                returnList = GetWalletReturnList(funName, ref flag, ref lstError, ref errCode);
            }

            if (flag)
            {
                foreach (var obj in returnList)
                {
                    #region 送台新錢包交易退款
                    WebAPI_Refund wsInput = new WebAPI_Refund()
                    {
                        ApiVersion = ApiVersion,
                        GUID = Guid.NewGuid().ToString().Replace("-", ""),
                        MerchantId = MerchantId,
                        POSId = obj.POSId,
                        StoreId = obj.StoreId,
                        AccountId = obj.AccountId,
                        SourceFrom = obj.SourceFrom,
                        StoreTransDate = obj.StoreTransDate,
                        StoreTransId = obj.StoreTransId,
                        TransId = obj.TransId,
                        RefundAmount = obj.ReturnAmt
                    };

                    var body = JsonConvert.SerializeObject(wsInput);
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = taishinWallet.GenerateSignCode(wsInput.MerchantId, utcTimeStamp, body, APIKey);

                    try
                    {
                        taishinWallet.DoRefund(wsInput, MerchantId, utcTimeStamp, SignCode, ref dbError, ref output);
                        logger.Trace($"DoRefund : {JsonConvert.SerializeObject(output)},dbError : {dbError} ");

                    }
                    catch (Exception ex)
                    {
                        exMsg = ex.Message;
                        logger.Error($"交易退款Exception: {ex.Message}");
                    }
                    #endregion
                    #region 交易回傳狀態更新
                    DateTime.TryParseExact(output?.Result?.TransDate, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime _TransDate);

                    SPInput_WalletReturn spInput = new SPInput_WalletReturn()
                    {
                        IDNO = obj.IDNO,
                        OrderNo = obj.Order_number,
                        WalletAccountID = obj.AccountId ?? "" ,
                        AuthSeq = obj.AuthSeq,
                        Amount = obj.ReturnAmt,
                        StoreTransId = obj.StoreTransId ?? "",
                        TransId = output?.Result?.TransId ?? "",
                        WalletBalance = output?.Result?.Amount ?? 0,
                        AuthFlg = output?.ReturnCode == "0000" ? 1 : -1,
                        AuthCode = output?.ReturnCode ?? "",
                        AuthMessage = output?.Message ?? exMsg ?? "",
                        Mode = 4,
                        PRGName = funName,
                        TradeType = obj.TradeType ?? "",
                        TransDate = _TransDate,
                        IsDuplicate = dbError == "ERR273" ? 1:0
                    };

                    UpdateWalletReturn(spInput, ref lstError, ref errCode);
                    logger.Trace($"UpdateWalletReturn : {JsonConvert.SerializeObject(spInput)},errCode : {errCode} ");
                    #endregion
                }

            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                BaseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            BaseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }


        /// 取得錢包退款清單
        private List<SPOutput_GetWalletReturn> GetWalletReturnList(string funName, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var capList = new List<SPOutput_GetWalletReturn>();

            SPInput_GetWalletReturn spInput = new SPInput_GetWalletReturn()
            {
                PRGName = funName
            };

            string SPName = "usp_WalletReturn_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetWalletReturn, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetWalletReturn, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref capList, ref ds, ref lstError);
            BaseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (capList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203"; //錢包退款找不到符合的訂單編號
                }
            }
            return capList;
        }

        /// 更新錢包退款狀態
        private bool UpdateWalletReturn(SPInput_WalletReturn input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_WalletReturn_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_WalletReturn, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_WalletReturn, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            BaseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;
        }

    }
}