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
using WebAPI.Utils;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Subscription;
using Domain.SP.Output.Subscription;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得訂閱制升轉列表
    /// </summary>
    public class GetUpSubsListController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetUpSubsList([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "DoGetUpSubsList";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetUpSubsList();
            var outputApi = new OAPI_GetUpSubsList();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetUpSubsList>(Contentjson);
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
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) || apiInput.MonProPeriod == 0)
                        {
                            flag = false;
                            errCode = "ERR257";
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
                    var spIn = new SPInput_GetUpSubsList()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonProjID = apiInput.MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays
                    };
                    trace.traceAdd("spIn", spIn);
                    var sp_re = msp.sp_GetUpSubsList(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);

                    if (sp_re != null)
                    {
                        if (sp_re.Cards != null && sp_re.Cards.Count() > 0)
                        {
                            var sour = sp_re.Cards;
                            var mixs = sour.Where(x => (x.CarHDHours > 0 || x.CarWDHours > 0) && x.MotoTotalMins > 0).ToList();
                            var nors = sour.Where(x => !mixs.Any(y => y.MonProjID == x.MonProjID && y.MonProPeriod == x.MonProPeriod && y.ShortDays == x.ShortDays)).ToList();

                            if (mixs != null && mixs.Count() > 0)
                                outputApi.MixCards = objUti.TTMap<List<SPOut_GetUpSubsList_Card>, List<OAPI_GetUpSubsList_Card>>(mixs);

                            if (nors != null && nors.Count() > 0)
                                outputApi.NorCards = objUti.TTMap<List<SPOut_GetUpSubsList_Card>, List<OAPI_GetUpSubsList_Card>>(nors);
                        }
                    }

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(188, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
