using Domain.Common;
using Domain.SP.Input.Wallet;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包歷程-儲值交易紀錄隱藏
    /// </summary>
    public class WalletStoreTradeHistoryHiddenController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoWalletStoreTradeHistoryHidden([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var wsp = new WalletSp();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoreTradeHistoryHiddenController";
            Int64 LogID = 0;
            var apiInput = new IAPI_WalletStoreTradeHistoryHidden();
            var outputApi = new OAPI_WalletStoreTradeHistoryHidden();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoreTradeHistoryHidden>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    if (apiInput == null || apiInput.SEQNO <= 0)
                    {
                        flag = false;
                        errMsg = "SEQNO必填";
                        errCode = "ERR257";
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

                    trace.FlowList.Add("防呆");
                    trace.traceAdd("InCk", new { flag, errCode });
                }

                #endregion

                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    trace.FlowList.Add("Token判斷");
                }
                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_WalletStoreTradeHistoryHidden()
                    {
                        LogID = LogID,
                        SEQNO = apiInput.SEQNO
                    };
                    trace.traceAdd("spIn", spIn);
                    flag = wsp.sp_WalletStoreTradeHistoryHidden(spIn, ref errCode);
                    trace.FlowList.Add("sp呼叫");
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(208, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}