using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Wallet;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包儲值-設定資訊
    /// </summary>
    public class GetWalletStoredMoneySetController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost()]
        public Dictionary<string, object> DoGetWalletStoredMoneySet(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetWalletStoredMoneySetController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetWalletStoredMoneySet();
            OAPI_GetWalletStoredMoneySet outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetWalletStoredMoneySet>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (apiInput == null || apiInput.StoreType < 1 || apiInput.StoreType > 4)
                {
                    flag = false;
                    errCode = "ERR900";//參數遺漏
                }

            }

            if (isGuest)
            {
                flag = false;
                errCode = "ERR101";
            }
            #endregion

            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            #region TB
            if (flag)
            {
                string spName = "usp_GetWalletStoredMoneySet_Q01";
                SPInput_GetWalletStoredMoneySet sPInput_GetWallet = new SPInput_GetWalletStoredMoneySet()
                {
                    LogID = LogID,
                    Token = Access_Token,
                    IDNO = IDNO,
                    StoreType = apiInput.StoreType
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetWalletStoredMoneySet, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetWalletStoredMoneySet, SPOutput_Base>(connetStr);
                List<SPOut_GetWalletStoredMoneySet> walletStoredMoneySets = new List<SPOut_GetWalletStoredMoneySet>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, sPInput_GetWallet, ref spOut, ref walletStoredMoneySets, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (walletStoredMoneySets.Count > 0)
                {
                    outputApi = new OAPI_GetWalletStoredMoneySet()
                    {
                        StoredMoneySet = walletStoredMoneySets.Select(t => new GetWalletStoredMoneySet
                        {
                            StoreType = t.StoreType,
                            StoreTypeDetail = t.StoreTypeDetail,
                            StoreLimit = t.StoreLimit,
                            StoreMax = t.StoreMax,
                            WalletBalance = t.WalletBalance,
                            Rechargeable = t.Rechargeable,
                            defSet = t.defSet,
                            QuickBtns = (string.IsNullOrWhiteSpace(t.QuickBtns)) ? new List<int>(0) : t.QuickBtns.Split(',').Select(Int32.Parse).ToList()
                        }).ToList()
                    };
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}