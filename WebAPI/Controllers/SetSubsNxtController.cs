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
using Domain.SP.Input.Bill;
using Domain.SP.Input.Subscription;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 設定自動續約
    /// </summary>
    public class SetSubsNxtController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoSetSubsNxt([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SetSubsNxtController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_SetSubsNxt();
            var outputApi = new OAPI_SetSubsNxt();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SetSubsNxt>(Contentjson);
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
                        var LstAutoSubs = new List<int> { 0, 1 };
                        if (!LstAutoSubs.Any(x => x == apiInput.AutoSubs))
                        {
                            flag = false;
                            errCode = "ERR910";//只可為0或1
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
                    var spIn = new SPInput_SetSubsNxt()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        NxtMonProjID = apiInput.MonProjID,
                        NxtMonProPeriod = apiInput.MonProPeriod,
                        NxtShortDays = apiInput.ShortDays,
                        AutoSubs = apiInput.AutoSubs
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    flag = msp.sp_SetSubsNxt(spIn, ref errCode);
                    trace.traceAdd("sp_re", flag);
                    trace.FlowList.Add("sp呼叫");
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(184, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
