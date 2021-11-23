using Domain.Common;
using Domain.SP.Input.Wallet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包歷史紀錄查詢
    /// </summary>
    public class WalletStoreTradeTransHistoryController : ApiController
    {
        [HttpPost]
        public Dictionary<string, object> DoWalletStoreTradeTransHistory([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoreTradeTransHistoryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            var apiInput = new IAPI_WalletStoreTradeTransHistory();
            var outputApi = new OAPI_WalletStoreTradeTransHistory();
            outputApi.TradeHis = new List<OAPI_WalletStoreTradeTrans>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            var wsp = new WalletSp();
            var wMap = new WalletMap();
            #endregion

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = JsonConvert.DeserializeObject<IAPI_WalletStoreTradeTransHistory>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    if (flag)
                    {
                        if (apiInput.SD == null || apiInput.ED == null)
                        {
                            flag = false;
                            errMsg = "SD, ED為必填";
                            errCode = "ERR257";//參數遺漏
                        }
                    }
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
                    var spIn = new SPInput_GetWalletStoreTradeTransHistory()
                    {
                        IDNO = IDNO,
                        Token = Access_Token,
                        SD = apiInput.SD,
                        ED = apiInput.ED,
                        LogID = LogID,
                    };
                    var sp_out = wsp.sp_GetWalletStoreTradeTransHistory(spIn, ref errCode);
                    flag = sp_out.flag;

                    if (flag)
                    {
                        if (sp_out.result != null && sp_out.result.Count() > 0)
                        {
                            var vmList = wMap.FromSPOut_GetWalletStoreTradeTransHistory(sp_out.result);
                            if (vmList != null && vmList.Count() > 0)
                                outputApi.TradeHis = vmList;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

            }

            #region 寫入錯誤Log
            if (!flag)
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