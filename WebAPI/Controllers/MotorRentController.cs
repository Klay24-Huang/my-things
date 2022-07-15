using Domain.Common;
using Domain.SP.Input.Car;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Subscription;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
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
        public Dictionary<string, object> DoMotorRent(Dictionary<string, object> value)
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
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            string IDNO = "";
            var _MotorRentObj = new List<OAPI_MotorRent_Param>();
            var Score = 100;  // 20210924 UPD BY YEH REASON:會員積分，預設100
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
                    if (flag == false)
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
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                if (errCode == "ERR101")    //訪客機制BYPASS
                {
                    flag = true;
                    errCode = "000000";
                }
            }
            #endregion

            #region 取得使用中訂閱制月租
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(IDNO) && apiInput.CarTrip != 2)
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
                        InUseMonth = sp_list.Where(x => x.IsMoto == 1 || x.IsMix == 1).ToList();
                }
            }
            #endregion

            #region 取得會員積分
            // 20210924 UPD BY YEH REASON:取得會員積分
            if (flag && !string.IsNullOrEmpty(IDNO))    // IDNO有值才撈積分
            {
                string spName = "usp_GetMemberScore_Q1";

                object[][] parms1 = {
                        new object[] {
                            IDNO,
                            1,
                            10,
                            LogID
                        }
                    };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (ds1.Tables.Count != 3)
                {
                    if (ds1.Tables.Count == 1)  // SP有回錯誤訊息以SP為主
                    {
                        baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR999";
                        errMsg = returnMessage;
                    }
                }
                else
                {
                    baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[2].Rows[0]["Error"]), ds1.Tables[2].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);

                    if (flag)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                            Score = Convert.ToInt32(ds1.Tables[0].Rows[0]["SCORE"]);
                        else
                            Score = 0;
                    }
                }
            }
            #endregion

            if (flag)
            {
                // 20210622 UPD BY YEH REASON:因應積分<60分只能用定價專案，取資料改去SP處理
                string SPName = "usp_GetMotorRent";
                SPInput_GetMotorRent spInput = new SPInput_GetMotorRent
                {
                    IDNO = IDNO,
                    ShowALL = apiInput.ShowALL.Value,
                    Latitude = apiInput.Latitude ?? 0,
                    Longitude = apiInput.Longitude ?? 0,
                    Radius = apiInput.Radius ?? 0,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetMotorRent, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMotorRent, SPOutput_Base>(connetStr);
                List<MotorRentObj> MotorList = new List<MotorRentObj>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref MotorList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    #region 春節專案使用
                    //春節限定，將R140專案移除
                    //var tempList = MotorList.Where(x => x.ProjID != "R140").ToList();
                    #endregion
                    List<AnyRentDiscountLabel> discountLabels = new List<AnyRentDiscountLabel>();
                    if (MotorList != null && MotorList.Count() > 0)
                    {
                        #region 取出優惠標籤
                        discountLabels = new CarRentCommon().GetDiscountLabelForAnyRentCars(
                            new Domain.SP.Input.Discount.SPInput_GetDiscountLabelForAnyRentCars
                            {
                                LogID = LogID,
                                CarNos = string.Join(",", MotorList.Select(o => o.CarNo.Trim()))
                            });
                        #endregion
                        _MotorRentObj = objUti.TTMap<List<MotorRentObj>, List<OAPI_MotorRent_Param>>(MotorList);
                    }
                    if (_MotorRentObj != null && _MotorRentObj.Count() > 0)
                    {
                        #region 企業客戶判斷
                        _MotorRentObj.ForEach(x => { x.TaxID = apiInput.CarTrip == 2 ? x.TaxID : ""; });
                        #endregion

                        #region 加入月租資訊
                        bool isSpring = new CarRentCommon().isSpring(SDate, EDate); //是否為春節時段

                        if (Score >= 60 && !isSpring)    // 20210924 UPD BY YEH REASON:積分>=60才可使用訂閱制
                        {
                            if (InUseMonth != null && InUseMonth.Count() > 0)
                            {
                                var f = InUseMonth.FirstOrDefault();

                                _MotorRentObj.ForEach(x =>
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

                        #region 加入優惠標籤
                        if (discountLabels != null && discountLabels.Count > 0)
                        {
                            _MotorRentObj.ForEach(x =>
                            {
                                x.DiscountLabel = discountLabels.Where(t => t.CarNo == x.CarNo)
                                   .Select(t => new DiscountLabel
                                   {
                                       LabelType = t.LabelType,
                                       GiveMinute = t.GiveMinute,
                                       Describe = $"{t.GiveMinute}分鐘優惠折抵",
                                   }).FirstOrDefault() ?? new DiscountLabel();
                            });
                        }
                        #endregion

                        OAnyRentAPI.MotorRentObj = _MotorRentObj;
                    }
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