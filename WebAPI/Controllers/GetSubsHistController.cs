using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Subscription;
using WebAPI.Utils;
using Domain.SP.Output.Subscription;

namespace WebAPI.Controllers
{
    public class GetSubsHistController : ApiController
    {
        /// <summary>
        /// 取得訂閱制歷史紀錄
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/GetSubsHist/DoGetSubsHist")]
        [HttpPost()]
        public Dictionary<string, object> DoGetSubsHist([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var map = new MonSunsVMMap();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetSubsHistController";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetSubsHist();
            var outputApi = new OAPI_GetSubsHist();
            outputApi.Hists = new List<OAPI_GetSubsHist_Param>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            if (value == null)
                value = new Dictionary<string, object>();

            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetSubsHist>(Contentjson);
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
                }

                #endregion

                #region token

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
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                }

                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_GetSubsHist()
                    {
                        IDNO = IDNO,
                        LogID = LogID,          
                        SetNow = apiInput.SetNow
                    };
                    trace.traceAdd("spIn", spIn);
                    var sp_list = msp.sp_GetSubsHist(spIn, ref errCode);
                    trace.traceAdd("sp_list", sp_list);
                    if (sp_list != null && sp_list.Count()>0)
                    {
                        var hasInvos = sp_list.Where(x => !string.IsNullOrWhiteSpace(x.invoiceCode)).ToList();
                        if (hasInvos != null & hasInvos.Count() > 0)
                        {
                            var hists = map.FromSPOut_GetSubsHist(hasInvos);
                            if (hists != null && hists.Count() > 0)
                                outputApi.Hists = hists;
                        }
                    }

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                trace.BaseMsg = ex.Message;
                errMsg = ex.Message;
                errCode = "ERR913";
                carRepo.AddTraceLog(189, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

        /// <summary>
        /// 刪除訂閱制歷史紀錄
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/GetSubsHist/DoDelSubsHist")]
        public Dictionary<string, object> DoDelSubsHist([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告

            var msp = new MonSubsSp();
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
            string funName = "GetSubsHistController";
            Int64 LogID = 0;
            var apiInput = new IAPI_DelSubsHist();
            var outputApi = new OAPI_DelSubsHist();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            if (value == null)
                value = new Dictionary<string, object>();

            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_DelSubsHist>(Contentjson);
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
                }

                #endregion

                #region token

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
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                }

                #endregion

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_DelSubsHist()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonthlyRentId = apiInput.MonthlyRentId,
                        SetNow = apiInput.SetNow
                    };
                    trace.traceAdd("spIn", spIn);
                    flag = msp.sp_DelSubsHist(spIn, ref errCode);
                    trace.traceAdd("sp_re", flag);
                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(189, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
