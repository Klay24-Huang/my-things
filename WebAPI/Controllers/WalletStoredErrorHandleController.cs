using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Hotai;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{

    public class WalletStoredErrorHandleController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private readonly string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private readonly string ApiVersion = ConfigurationManager.AppSettings["TaishinWalletApiVersion"].ToString();
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonFunc BaseVerify { get; set; }
        /// <summary>
        /// 錢包儲值錯誤處理
        /// </summary>
        [HttpPost]
        public Dictionary<string, object> DoWalletStoredErrorHandle()
        {
            #region 初始宣告
            var wsp = new WalletSp();
            var objOutput = new Dictionary<string, object>();    //輸出
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoredErrorHandleController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            NullOutput apiOutput = new NullOutput();
            Token token = null;
            BaseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<SPOutput_GetWalletStoredErrorList> errList = null;
            WebAPIOutput_StoreValueCreateAccount output = null;
            #endregion
            #region TB
            //寫入API Log
            string ClientIP = BaseVerify.GetClientIp(Request);
            bool flag = BaseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);

            if (flag)
            {
                //儲值錯誤清單
                errList = GetWalletStoredErrorList(funName, ref flag, ref lstError, ref errCode);
            }

            if (flag)
            {
                foreach (var item in errList)
                {
                    #region 台新錢包儲值
                    if (flag)
                    {
                        DateTime NowTime = DateTime.Now;
                        var wallet = new WebAPI_CreateAccountAndStoredMoney()
                        {
                            ApiVersion = ApiVersion,
                            GUID = Guid.NewGuid().ToString().Replace("-", ""),
                            MerchantId = MerchantId,
                            POSId = "",
                            StoreId = "1",
                            StoreName = "",
                            StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                            StoreTransId = string.Format("{0}{1}", item.ID, NowTime.ToString("MMddHHmmss")),
                            MemberId = item.MemberId,
                            Name = item.Name,
                            PhoneNo = item.PhoneNo,
                            Email = item.Email,
                            ID = item.IsForeign == 1 ? "" : item.ID,
                            AccountType = item.AccountType,
                            AmountType = item.AmountType,
                            CreateType = item.CreateType,
                            Amount = item.Amount,
                            Bonus = item.Bonus,
                            BonusExpiredate = item.BonusExpiredate,
                            SourceFrom = item.SourceFrom
                        };

                        var body = JsonConvert.SerializeObject(wallet);
                        TaishinWallet WalletAPI = new TaishinWallet();
                        string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                        string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                        try
                        {
                            flag = WalletAPI.DoStoreValueCreateAccount(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            logger.Error("WalletStoredErrorListloop Error:" + ex.Message);
                        }
                    }
                    #endregion
                    #region 寫入錢包
                    if (flag)
                    {
                        string formatString = "yyyyMMddHHmmss";
                        string cardNo = item.CardNumber.Substring((item.CardNumber.Length - 5) > 0 ? item.CardNumber.Length - 5 : 0);
                        SPInput_WalletStore spInput_Wallet = new SPInput_WalletStore()
                        {
                            IDNO = item.ID,
                            WalletMemberID = output.Result.MemberId,
                            WalletAccountID = output.Result.AccountId,
                            Status = Convert.ToInt32(output.Result.Status),
                            Email = output.Result.Email,
                            PhoneNo = output.Result.PhoneNo,
                            StoreAmount = item.Amount,
                            WalletBalance = output.Result.Amount,
                            CreateDate = DateTime.ParseExact(output.Result.CreateDate, formatString, null),
                            LastTransDate = DateTime.ParseExact(output.Result.TransDate, formatString, null),
                            LastStoreTransId = output.Result.StoreTransId,
                            LastTransId = output.Result.TransId,
                            TaishinNO = item.BankTradeNo,
                            TradeType = item.TradeType,
                            TradeKey = cardNo,
                            PRGName = funName,
                            Mode = 1,
                            InputSource = 2,
                            LogID = LogID
                        };

                        flag = wsp.sp_WalletStore(spInput_Wallet, ref errCode);
                    }
                    #endregion
                    #region 更新儲值錯誤LOG
                    SPInput_WalletStoredErrorHandle input = new SPInput_WalletStoredErrorHandle()
                    {
                        Seqno = item.SEQNO,
                        ProcessStatus = flag ? 1 : 2,
                        ReturnCode = output?.ReturnCode ?? "",
                        ExceptionData = output?.ExceptionData ?? "",
                        Message = output?.Message ?? "",
                        PRGName = funName
                    };
                    flag = UpdWalletStoredErrorLog(input, ref lstError, ref errCode);
                    #endregion

                }
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                BaseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            BaseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        //儲值錯誤清單
        private List<SPOutput_GetWalletStoredErrorList> GetWalletStoredErrorList(string funName, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var errList = new List<SPOutput_GetWalletStoredErrorList>();

            SP_Input_CTBCCapBase spInput = new SP_Input_CTBCCapBase()
            {
                PRGName = funName
            };

            string SPName = "usp_WalletStoredErrorHandle_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SP_Input_CTBCCapBase, SPOutput_Base> sqlHelpQuery = new SQLHelper<SP_Input_CTBCCapBase, SPOutput_Base>(connetStr);

            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref errList, ref ds, ref lstError);
            BaseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (errList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203"; //找不到符合的訂單編號
                }
            }
            return errList;
        }

        //更新錢包儲值紀錄
        private bool UpdWalletStoredErrorLog(SPInput_WalletStoredErrorHandle input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_WalletStoredErrorHandle_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_WalletStoredErrorHandle, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_WalletStoredErrorHandle, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            BaseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
    }
}
