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

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取的專案群組
    /// </summary>
    public class GetMonthGroupController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetMonthGroup([FromBody] Dictionary<string, object> value)
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
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMonthGroupController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetMonthGroup();
            var outputApi = new OAPI_GetMonthGroup();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            outputApi.MonCards = new List<GetMonthGroup_MonCardParam>();
            string IDNO = "";

            #endregion

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMonthGroup>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
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
                    var spIn = new SPInput_GetMonthGroup()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonProjID = apiInput.MonProjID
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var sp_List = msp.sp_GetMonthGroup(spIn, ref errCode);
                    trace.traceAdd("sp_List", sp_List);
                    if (sp_List != null && sp_List.Count() > 0)
                    {
                        var cards = (from a in sp_List
                                     orderby a.MonProPeriod
                                     select new GetMonthGroup_MonCardParam
                                     {
                                         MonProjID = a.MonProjID,
                                         MonProjNM = a.MonProjNM,
                                         MonProPeriod = a.MonProPeriod,
                                         ShortDays = a.ShortDays,
                                         PeriodPrice = a.PeriodPrice,
                                         IsMoto = a.IsMoto,
                                         CarWDHours = a.CarWDHours,
                                         CarHDHours = a.CarHDHours,
                                         MotoTotalMins = Convert.ToInt32(a.MotoTotalMins),
                                         WDRateForCar = a.WDRateForCar,
                                         HDRateForCar = a.HDRateForCar,
                                         WDRateForMoto = a.WDRateForMoto,
                                         HDRateForMoto = a.HDRateForMoto,
                                         IsDiscount = a.IsDiscount,
                                         IsMix = a.IsMix,        //20210525 ADD BY ADAM REASON.增加城市車手
                                         //20210616 ADD BY ADAM 
                                         //UseUntil = a.UseUntil.ToString("yyyy/MM/dd")
                                         UseUntil = apiInput.Mode == "1" ? a.UseUntil.ToString("yyyy/MM/dd HH:mm") :
                                                a.UseUntil.ToString("HHmm") == "0000" ? a.UseUntil.AddMinutes(-1).ToString("yyyy/MM/dd HH:mm") : a.UseUntil.ToString("yyyy/MM/dd HH:mm")
                                     }).ToList();

                        outputApi.MonProDisc = sp_List.FirstOrDefault().MonProDisc;
                        //outputApi.IsOrder = sp_List.Where(x => x.IsOrder == 1).ToList().Count() > 0 ? 1:0;
                        outputApi.MonCards = cards;
                        trace.traceAdd("outputApi", outputApi);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(179, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
