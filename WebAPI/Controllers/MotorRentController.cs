using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Subscription;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class MotorRentController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doGetNormalRent(Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MotorRentController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MotorRent apiInput = null;
            var OAnyRentAPI = new OAPI_MotorRent();
            OAnyRentAPI.MotorRentObj = new List<OAPI_MotorRent_Param>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            string IDNO = "";
            var _MotorRentObj = new List<OAPI_MotorRent_Param>();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MotorRent>(Contentjson);
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

            #region Token判斷
            //if (flag && isGuest == false)
            //{
            //    string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenOnlyToken);
            //    SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
            //    {
            //        LogID = LogID,
            //        Token = Access_Token
            //    };
            //    SPOutput_Base spOut = new SPOutput_Base();
            //    SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base>(connetStr);
            //    flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            //    baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            //}

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
                    IDNO = token_re.IDNO ?? "";
                }
            }

            #endregion

            #region TB

            //取得機車使用中訂閱制月租
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
                        IsMoto = -1
                    };
                    var sp_list = new MonSubsSp().sp_GetNowSubs(sp_in, ref errCode);
                    if (sp_list != null && sp_list.Count() > 0)
                        InUseMonth = sp_list.Where(x=>x.IsMoto == 1 || x.IsMix == 1).ToList();
                }
            }

            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
                List<MotorRentObj> AllCars = new List<MotorRentObj>();
                if (apiInput.ShowALL == 1)
                {
                    AllCars = _repository.GetAllMotorRent();
                }
                else
                {
                    AllCars = _repository.GetAllMotorRent(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }

                if (AllCars != null && AllCars.Count > 0)
                {
                    AllCars.ForEach(x =>
                    {
                        x.Power = Convert.ToInt32(x.Power);
                        x.RemainingMileage = Convert.ToInt32(x.RemainingMileage);
                    });
                }

                //春節限定，將R140專案移除
                var tempList = AllCars.Where(x => x.ProjID != "R140").ToList();

                if(tempList != null && tempList.Count()>0)
                    _MotorRentObj = objUti.TTMap<List<MotorRentObj>, List<OAPI_MotorRent_Param>>(tempList);               

                if (_MotorRentObj != null && _MotorRentObj.Count() > 0)
                {
                    #region 加入月租資訊
                    if (InUseMonth != null && InUseMonth.Count() > 0)
                    {
                        var finalOut = new List<OAPI_MotorRent_Param>();
                        _MotorRentObj.ForEach(x => {
                            finalOut.Add(x);
                            InUseMonth.ForEach(y =>
                            {
                                var newItem = objUti.Clone(x);
                                newItem.MonthlyRentId = y.MonthlyRentId;
                                newItem.MonProjNM = y.MonProjNM;
                                newItem.CarWDHours = y.WorkDayHours;
                                newItem.CarHDHours = y.HolidayHours;
                                newItem.MotoTotalMins = Convert.ToInt32(y.MotoTotalMins);
                                newItem.WDRateForCar = y.WorkDayRateForCar;
                                newItem.HDRateForCar = y.HoildayRateForCar;
                                newItem.WDRateForMoto = y.WorkDayRateForMoto;
                                newItem.HDRateForMoto = y.HoildayRateForMoto;
                                finalOut.Add(newItem);
                            });
                        });
                        _MotorRentObj = finalOut;
                    }
                    #endregion
                    OAnyRentAPI.MotorRentObj = _MotorRentObj;
                }                
            }
            
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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