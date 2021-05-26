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

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得我的方案
    /// </summary>
    public class GetMySubsController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetMySubs([FromBody] Dictionary<string, object> value)
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
            string funName = "GetMySubsController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetMySubs();
            var outputApi = new OAPI_GetMySubs();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int MonType = 2;//暫時只有訂閱制月租

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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMySubs>(Contentjson);
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
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) ||apiInput.MonProPeriod == 0)
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
                    var spIn = new SPInput_GetMySubs()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        MonType = MonType
                    };
                    trace.traceAdd("spIn", spIn);
                    //取出月租列表
                    var sp_re = msp.sp_GetMySubs(spIn, ref errCode);
                    trace.traceAdd("sp_re", sp_re);
                    if (sp_re != null)
                    {  
                        if (sp_re.Months != null && sp_re.Months.Count() > 0)
                        {
                            var months = (from a in sp_re.Months
                                            select new OAPI_GetMySubs_Month
                                            {
                                                MonProjID = a.MonProjID,
                                                MonProjNM = a.MonProjNM,
                                                MonProPeriod = a.MonProPeriod,
                                                ShortDays = a.ShortDays,
                                                CarWDHours = a.WorkDayHours,
                                                CarHDHours = a.HolidayHours,
                                                MotoTotalMins = Convert.ToInt32(a.MotoTotalHours),  //20210525 ADD BY ADAM REASON.改為int
                                                WDRateForCar = a.WorkDayRateForCar,
                                                HDRateForCar  = a.HoildayRateForCar,
                                                WDRateForMoto = a.WorkDayRateForMoto,
                                                HDRateForMoto = a.HoildayRateForMoto,
                                                StartDate = a.StartDate.ToString("MM/dd"),
                                                EndDate = a.EndDate.ToString("HHmm") == "0000" ? a.EndDate.AddMinutes(-1).ToString("MM/dd HH:mm") : a.EndDate.ToString("MM/dd HH:mm"),
                                                MonthStartDate = a.MonthStartDate.ToString("yyyy/MM/dd"),
                                                MonthEndDate = a.MonthEndDate.ToString("yyyy/MM/dd"),
                                                NxtMonProPeriod = a.NxtMonProPeriod,
                                                IsMix = a.IsMix,                                                
                                                IsUpd = a.IsUpd,
                                                SubsNxt = a.SubsNxt,
                                                IsChange = a.IsChange,
                                                IsPay = a.IsPay
                                            }).ToList();

                            var NowMon = months.Where(x => x.MonProjID == apiInput.MonProjID
                                && x.MonProPeriod == apiInput.MonProPeriod && x.ShortDays == apiInput.ShortDays).ToList();

                            if (NowMon != null && NowMon.Count() > 0)
                                outputApi.Month = NowMon.FirstOrDefault();
                            else
                            {
                                flag = false;
                                errMsg = "查無指定月租";
                                errCode = "ERR909";//專案不存在
                            }
                        }
                        else
                        {
                            flag = false;
                            errMsg = "查無指定月租";
                            errCode = "ERR909";//專案不存在
                        }
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
                carRepo.AddTraceLog(183, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
