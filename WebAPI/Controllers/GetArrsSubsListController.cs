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

namespace WebAPI.Controllers
{
    public class GetArrsSubsListController : ApiController
    {
        /// <summary>
        /// 訂閱牌卡制欠費查詢
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/GetArrsSubsList/DoGetArrsSubsList")]
        [HttpPost()]
        public Dictionary<string, object> DoGetArrsSubsList([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var map = new MonSunsVMMap();
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
            var apiInput = new IAPI_GetArrsSubsList();
            var outputApi = new OAPI_GetArrsSubsList();
            outputApi.Arrs = new List<OAPI_GetArrsSubsList_arrs>();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetArrsSubsList>(Contentjson);
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
                    var spIn = new SPInput_GetArrsSubsList()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonProjID = apiInput.MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays
                    };
                    trace.traceAdd("spIn", spIn);
                    var sp_re = msp.sp_GetArrsSubsList(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);
                    if (sp_re != null )
                    {
                        if(sp_re.DateRange != null)
                        {
                            var reDate = sp_re.DateRange;
                            if (!string.IsNullOrWhiteSpace(reDate.SD))
                                outputApi.SD = Convert.ToDateTime(reDate.SD).ToString("yyyy/MM/dd");

                            if (!string.IsNullOrWhiteSpace(reDate.ED))
                                outputApi.ED = Convert.ToDateTime(reDate.ED).ToString("yyyy/MM/dd");
                        }

                        if(sp_re.Arrs != null && sp_re.Arrs.Count() > 0)
                        {
                            var arrs = sp_re.Arrs;
                            outputApi.ProjNm = arrs.FirstOrDefault().MonProjNM;
                            outputApi.Arrs = map.FromSPOut_GetArrsSubsList(arrs);
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

    }
}
