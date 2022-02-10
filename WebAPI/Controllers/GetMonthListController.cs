using Domain.Common;
using Domain.SP.Input.Rent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得訂閱制月租列表/我的所有方案
    /// </summary>
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
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMonthListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetMonthList();

            var outApiAllCards = new OAPI_AllMonthList_Car();
            outApiAllCards.NorMonCards = new List<MonCardParam>();
            outApiAllCards.MixMonCards = new List<MonCardParam>();

            var outApiAllMotos = new OAPI_AllMonthList_Moto();
            outApiAllMotos.NorMonCards = new List<MonCardParam>();

            var outApiMyCards = new OAPI_MyMonthList();

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int ReMode = 0;//全部月租(1), 我的方案(2)
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

                    List<int> IsMotorCk = new List<int>() { 0, 1 };
                    if (apiInput != null)
                    {
                        int IsMoto = apiInput.IsMoto;
                        if (!IsMotorCk.Any(x => x == IsMoto))
                        {
                            flag = false;
                            errCode = "ERR257";
                        }
                        else
                            outApiAllCards.IsMotor = IsMoto;

                        if (flag)
                            ReMode = apiInput.ReMode;
                    }
                }
                #endregion

                #region TB
                #region Token
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                }
                #endregion
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
                        if (ReMode == 0)//未設定回傳模式會自動分類
                        {
                            if (sp_re.MyMonths != null && sp_re.MyMonths.Count() > 0)
                                ReMode = 2;//我的方案
                            else
                                ReMode = 1;//月租列表
                        }

                        if (ReMode == 2)
                        {
                            var myCards = sp_re.MyMonths;
                            if (myCards != null && myCards.Count() > 0)
                            {
                                var myCars = myCards.Where(x => x.IsMoto == 0).ToList();
                                var myMotos = myCards.Where(x => x.IsMoto == 1).ToList();

                                if (myCars != null && myCars.Count() > 0)
                                    outApiMyCards.MyCar = map.FromSPOutput_GetMonthList_My(myCars).FirstOrDefault();
                                if (myMotos != null && myMotos.Count() > 0)
                                    outApiMyCards.MyMoto = map.FromSPOutput_GetMonthList_My(myMotos).FirstOrDefault();
                            }

                            outApiMyCards.ReMode = 2;
                            trace.traceAdd("outApiMyCards", outApiMyCards);
                        }
                        else
                        {
                            var allmons = map.FromSPOutput_GetMonthList_Month(sp_re.AllMonths.Where(x => x.IsMoto == apiInput.IsMoto).ToList());//區分汽機車
                            var mixCards = allmons.Where(x => x.IsMix == 1).ToList();    //20210715 調整城市車手判斷邏輯
                            var norCards = allmons.Where(x => !mixCards.Any(y => y.MonProjID == x.MonProjID && y.MonProPeriod == x.MonProPeriod && y.ShortDays == x.ShortDays)).ToList();
                            //20210715 ADD BY ADAM REASON.針對城市車手調整機車無時數時
                            mixCards.ForEach(x =>
                            {
                                x.MotoTotalMins = x.MotoTotalMins == -999 ? 0 : x.MotoTotalMins;
                            });

                            if (apiInput.IsMoto == 0)
                            {
                                if (mixCards != null && mixCards.Count() > 0)
                                    outApiAllCards.MixMonCards = mixCards;
                                if (norCards != null && norCards.Count() > 0)
                                    outApiAllCards.NorMonCards = norCards;
                                outApiAllCards.IsMotor = 0;
                                outApiAllCards.ReMode = 1;
                            }
                            else if (apiInput.IsMoto == 1)
                            {
                                if (norCards != null && norCards.Count() > 0)
                                    outApiAllMotos.NorMonCards = norCards;
                                outApiAllMotos.IsMotor = 1;
                                outApiAllMotos.ReMode = 1;
                            }

                            trace.traceAdd("outApiAllCards", outApiAllCards);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(178, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            if (ReMode == 2)
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApiMyCards, token);
            else
            {
                if (apiInput.IsMoto == 1)
                    baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApiAllMotos, token);
                else
                    baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApiAllCards, token);
            }

            return objOutput;
            #endregion        
        }
    }
}