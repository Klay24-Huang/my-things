using Domain.Common;
using Domain.SP.Input.Car;
using Domain.SP.Input.Common;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Subscription;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 路邊租還
    /// </summary>
    public class AnyRentController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doGetNormalRent(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "AnyRentController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_AnyRent apiInput = null;
            var OAnyRentAPI = new OAPI_AnyRent();
            OAnyRentAPI.AnyRentObj = new List<OAPI_AnyRent_Param>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            var _AnyRentObj = new List<OAPI_AnyRent_Param>();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_AnyRent>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    flag = apiInput.ShowALL.HasValue;
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        if (apiInput.ShowALL.Value == 0)
                        {
                            if (!apiInput.Latitude.HasValue || !apiInput.Longitude.HasValue || !apiInput.Radius.HasValue)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
                    }
                }
            }
            #endregion

            #region TB
            //Token判斷
            //20201109 ADD BY ADAM REASON.TOKEN判斷修改
            if (flag && Access_Token_string.Split(' ').Length >= 2)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token_string.Split(' ')[1].ToString()
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                //訪客機制BYPASS
                if (spOut.ErrorCode == "ERR101")
                {
                    flag = true;
                    spOut.ErrorCode = "";
                    spOut.Error = 0;
                    errCode = "000000";
                }
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            //取得汽車使用中訂閱制月租
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(IDNO))
                {
                    var sp_in = new SPInput_GetNowSubs()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        SD = SDate,
                        ED = EDate,
                        IsMoto = 0
                    };
                    var sp_list = new MonSubsSp().sp_GetNowSubs(sp_in, ref errCode);
                    if (sp_list != null && sp_list.Count() > 0)
                        InUseMonth = sp_list;
                }
            }

            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);

                double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
                latlngLimit = _repository.GetAround(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);

                string spName = new ObjType().GetSPName(ObjType.SPType.GetAnyRentCar);
                SPInput_GetAnyRentCar spCarStatusInput = new SPInput_GetAnyRentCar()
                {
                    IDNO = IDNO,
                    ShowALL = apiInput.ShowALL.Value,
                    MinLatitude = latlngLimit[0],
                    MinLongitude = latlngLimit[1],
                    MaxLatitude = latlngLimit[2],
                    MaxLongitude = latlngLimit[3],
                    LogID = LogID,
                };
                SPOutput_Base SPOutputBase = new SPOutput_Base();
                SQLHelper<SPInput_GetAnyRentCar, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetAnyRentCar, SPOutput_Base>(connetStr);
                List<AnyRentObj> ListOut = new List<AnyRentObj>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spCarStatusInput, ref SPOutputBase, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);

                //春節限定，將R139專案移除，並將R139的價格給原專案
                List<AnyRentObj> ListOutCorrect = new List<AnyRentObj>();
                ListOutCorrect = ListOut.Where(x => x.ProjID != "R139").ToList();

                var TempR139List = ListOut.Where(x => x.ProjID == "R139").ToList();
                if (TempR139List != null)
                {
                    foreach (var temp in TempR139List)
                    {
                        var Modify = ListOutCorrect.Where(x => x.CarNo == temp.CarNo).FirstOrDefault();
                        if (Modify != null)
                        {
                            Modify.Rental = temp.Rental;
                        }
                    }
                }

                if(ListOutCorrect!= null && ListOutCorrect.Count() > 0)
                    _AnyRentObj = objUti.TTMap<List<AnyRentObj>, List<OAPI_AnyRent_Param>>(ListOutCorrect);

                #region 加入月租資訊

                if(_AnyRentObj != null && _AnyRentObj.Count() > 0)
                {
                    if(InUseMonth != null && InUseMonth.Count() > 0)
                    {
                        var f = InUseMonth.FirstOrDefault();
                        _AnyRentObj.ForEach(x =>
                        {
                            x.MonthlyRentId = f.MonthlyRentId;
                            x.MonProjNM = f.MonProjNM;
                            x.CarWDHours = f.WorkDayHours;
                            x.CarHDHours = f.HolidayHours;
                            x.MotoTotalMins = Convert.ToInt32(f.MotoTotalMins);
                            x.WDRateForCar = f.WorkDayRateForCar;
                            x.HDRateForCar = f.HoildayRateForCar;
                            x.WDRateForMoto = f.WorkDayRateForMoto;
                            x.HDRateForMoto = f.HoildayRateForMoto;
                        });
                    }
                }

                #endregion

                OAnyRentAPI = new OAPI_AnyRent()
                {
                    AnyRentObj = _AnyRentObj
                };
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, OAnyRentAPI, token);
            return objOutput;
            #endregion
        }
    }
}