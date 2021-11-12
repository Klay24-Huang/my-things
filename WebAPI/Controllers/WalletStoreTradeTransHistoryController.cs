using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Models.BillFunc;
using Newtonsoft.Json;
using Domain.SP.Input.Wallet;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包歷史紀錄
    /// </summary>
    public class WalletStoreTradeTransHistoryController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoWalletStoreTradeTransHistory([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var wsp = new WalletSp();
            var wMap = new WalletMap();
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
            string funName = "WalletStoreTradeTransHistory";
            Int64 LogID = 0;
            var apiInput = new IAPI_WalletStoreTradeTransHistory();
            var outputApi = new OAPI_WalletStoreTradeTransHistory();
            outputApi.TradeHis = new List<OAPI_WalletStoreTradeTrans>();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoreTradeTransHistory>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }

                    if (flag) 
                    {
                        if (apiInput.SD == null || apiInput.ED == null)
                        {
                            flag = false;
                            errMsg = "SD, ED為必填";
                            errCode = "ERR257";//參數遺漏
                        }
                    }

                    trace.FlowList.Add("防呆");
                    trace.traceAdd("InCk", new { flag, errCode });
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                    trace.traceAdd("TokenCk", new { flag, errCode });
                }

                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_GetWalletStoreTradeTransHistory()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        SD = apiInput.SD,
                        ED = apiInput.ED
                    };
                    trace.traceAdd("spIn", spIn);
                    var sp_list = wsp.sp_GetWalletStoreTradeTransHistory(spIn, ref errCode);
                    if (sp_list != null && sp_list.Count() > 0)
                    {
                        var vmList = wMap.FromSPOut_GetWalletStoreTradeTransHistory(sp_list);
                        if (vmList != null && vmList.Count() > 0) 
                            outputApi.TradeHis = vmList;

                        trace.traceAdd("sp_list", sp_list);
                    }
                   
                    trace.FlowList.Add("sp呼叫");
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(206, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    
    }
}
