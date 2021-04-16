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

namespace WebAPI.Controllers
{
    public class GetMonthListController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetMonthList([FromBody] Dictionary<string, object> value)
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
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMonthListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetMonthList();

            var outApiAllCards = new OAPI_AllMonthList();
            outApiAllCards.NorMonCards = new List<MonCardParam>();
            outApiAllCards.MixMonCards = new List<MonCardParam>();

            var outApiMyCards = new OAPI_MyMonthList();
            outApiMyCards.MyCards = new List<MonCardParam>();
            outApiMyCards.OtherCards = new List<MonCardParam>();

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            int ReMode = 1;//全部月租(1), 我的方案(2)

            #endregion

            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMonthList>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    List<int> IsMotorCk = new List<int>() {0, 1 };
                    if(apiInput != null)
                    {
                        int IsMoto = apiInput.IsMoto;
                        if (!IsMotorCk.Any(x => x == IsMoto))
                        {
                            flag = false;
                            errCode = "ERR257";
                        }
                        else
                            outApiAllCards.IsMotor = IsMoto;
                    }
                }

                #endregion

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

                #region TB

                if (flag)
                {
                    var spIn = new SPInput_GetMonthList()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonType = 2 //暫時只有訂閱制月租
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var sp_re = msp.sp_GetMonthList(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);
                    if (sp_re != null)
                    {
                        if (sp_re.MyMonths != null && sp_re.MyMonths.Count()>0)
                            ReMode = 2;//我的方案
                        else
                            ReMode = 1;//月租列表

                        if (ReMode == 2)
                        {
                            var myCards = sp_re.MyMonths;
                            if (myCards != null && myCards.Count() > 0)
                                outApiMyCards.MyCards = map.FromSPOutput_GetMonthList_Month(myCards);

                            var otherCards = sp_re.AllMonths.Where(x =>
                              !myCards.Any(y => y.MonProjID == x.MonProjID)).ToList();
                            if (otherCards != null && otherCards.Count() > 0)
                                outApiMyCards.OtherCards = map.FromSPOutput_GetMonthList_Month(otherCards);

                            outApiMyCards.ReMode = 2;
                            trace.traceAdd("outApiMyCards", outApiMyCards);
                        }
                        else
                        {
                            var allmons = map.FromSPOutput_GetMonthList_Month(sp_re.AllMonths.Where(x=>x.IsMoto == apiInput.IsMoto).ToList());//區分汽機車
                            var mixCards = allmons.Where(x =>
                               (x.CarWDHours > 0 || x.CarHDHours > 0) && x.MotoTotalMins > 0).ToList();
                            var norCards = allmons.Where(x =>
                               !mixCards.Any(y => y.MonProjID == x.MonProjID && y.MonProPeriod == x.MonProPeriod && y.ShortDays == x.ShortDays)).ToList();

                            if (mixCards != null && mixCards.Count() > 0)
                                outApiAllCards.MixMonCards = mixCards;
                            if (norCards != null && norCards.Count() > 0)
                                outApiAllCards.NorMonCards = norCards;

                            outApiAllCards.ReMode = 1;
                            trace.traceAdd("outApiAllCards", outApiAllCards);
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {                
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(178,funName, eumTraceType.exception,trace);
            }

            #region 輸出

            if(ReMode == 2)
               baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApiMyCards, token);
            else
               baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApiAllCards, token);

            return objOutput;
            #endregion        
        }
    }
}
