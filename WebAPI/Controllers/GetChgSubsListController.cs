using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Rent;
using Newtonsoft.Json;
using Domain.SP.Input.Subscription;
using Domain.SP.Output.Subscription;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 變更下期列表
    /// </summary>
    public class GetChgSubsListController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetChgSubsList([FromBody] Dictionary<string, object> value)
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
            string funName = "GetChgSubsListController";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetChgSubsList();
            var outputApi = new OAPI_GetChgSubsList();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetChgSubsList>(Contentjson);
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
                    var spIn = new SPInput_GetChgSubsList()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonProjID = apiInput.MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var sp_re = msp.sp_GetChgSubsList(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);

                    if (sp_re != null)
                    {
                        if (sp_re.NowCard != null)
                            outputApi.MyCard = objUti.TTMap<SPOut_GetChgSubsList_Card, OPAI_GetChgSubsList_Card>(sp_re.NowCard);

                        if(sp_re.OtrCards != null && sp_re.OtrCards.Count()>0)
                            outputApi.OtrCards = objUti.TTMap<List<SPOut_GetChgSubsList_Card>, List<OPAI_GetChgSubsList_Card>>(sp_re.OtrCards);
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR908";//sp錯誤
                    }

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(187, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
