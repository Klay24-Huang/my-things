using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using NLog;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】修改合約前取得資料(2021新版)
    /// </summary>
    public class BE_GetOrderModifyInfoNewController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】修改合約前取得資料
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_GetOrderModifyInfo(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_GetOrderModifyInfoNewController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_GetOrderModifyInfo apiInput = null;
            OAPI_BE_GetOrderModifyInfoNew apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            int OldCarPoint = 0;
            int OldMotorPoint = 0;
            DateTime SD = DateTime.Now, ReturnDate = DateTime.Now;
            List<BE_CarScheduleTimeLog> lstOrder = null;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_GetOrderModifyInfo>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if (apiInput.OrderNo.IndexOf("H") < 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                        if (flag)
                        {
                            if (tmpOrder <= 0)
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
            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderInfoBeforeModifyV3);
                SPOutput_BE_GetOrderInfoBeforeModify spOut = new SPOutput_BE_GetOrderInfoBeforeModify();
                SPInput_BE_GetOrderInfoBeforeModify spInput = new SPInput_BE_GetOrderInfoBeforeModify()
                {
                    OrderNo = tmpOrder,
                    UserID = apiInput.UserID,
                    LogID = LogID
                };

                SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetOrderInfoBeforeModify> sqlHelp = new SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetOrderInfoBeforeModify>(connetStr);
                List<BE_GetOrderModifyDataNewV2> OrderDataLists = new List<BE_GetOrderModifyDataNewV2>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref OrderDataLists, ref ds, ref lstError);
                new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    apiOutput = new OAPI_BE_GetOrderModifyInfoNew()
                    {
                        LastOrderData = new LastOrderInfo()
                        {
                            LastEndMile = spOut.LastEndMile,
                            LastStartTime = spOut.LastStartTime,
                            LastStopTime = spOut.LastStopTime
                        },
                        ModifyLog = new ModifyInfo()
                        {
                            hasModify = spOut.hasModify,
                            ModifyTime = spOut.ModifyTime,
                            ModifyUserID = spOut.ModifyUserID
                        },

                        OrderData = new BE_GetOrderModifyDataNewV2()
                    };
                    if (OrderDataLists != null)
                    {
                        if (OrderDataLists.Count > 0)
                        {
                            apiOutput.OrderData = OrderDataLists[0];
                            OldCarPoint = OrderDataLists[0].gift_point;
                            OldMotorPoint = OrderDataLists[0].gift_motor_point;
                        }
                    }
                }
            }
            #endregion

            #region TB
            //開始送短租查詢
            if (flag)
            {
                WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR270Query(apiOutput.OrderData.IDNO, ref wsOutput);

                if (flag)
                {
                    int giftLen = wsOutput.Data.Length;

                    if (giftLen > 0)
                    {
                        //OAPI_BonusQuery objBonus = new OAPI_BonusQuery();
                        //objBonus.BonusObj = new List<BonusData>();
                        int TotalGiftPoint = 0;
                        int TotalLastPoint = 0;
                        int TotalGiftPointCar = 0;
                        int TotalGiftPointMotor = 0;
                        int TotalLastPointCar = 0;
                        int TotalLastPointMotor = 0;
                        int TotalLastTransPointCar = 0;
                        int TotalLastTransPointMotor = 0;

                        for (int i = 0; i < giftLen; i++)
                        {
                            DateTime tmpDate;
                            int tmpPoint = 0;
                            bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                            bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                            if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                            {
                                //  totalPoint += tmpPoint;
                                BonusData objPoint = new BonusData()
                                {
                                    //20201021 ADD BY ADAM REASON.補上流水號
                                    SEQNO = wsOutput.Data[i].SEQNO,

                                    PointType = (wsOutput.Data[i].GIFTTYPE == "01") ? 0 : 1,
                                    EDATE = (wsOutput.Data[i].EDATE == "") ? "" : (wsOutput.Data[i].EDATE.Split(' ')[0]).Replace("/", "-"),
                                    GIFTNAME = wsOutput.Data[i].GIFTNAME,
                                    GIFTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].GIFTPOINT) ? "0" : wsOutput.Data[i].GIFTPOINT,
                                    LASTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? "0" : wsOutput.Data[i].LASTPOINT,
                                    AllowSend = string.IsNullOrEmpty(wsOutput.Data[i].RCVFLG) ? 0 : ((wsOutput.Data[i].RCVFLG == "Y") ? 1 : 0)
                                };
                                if (objPoint.PointType == 0)
                                {
                                    if (!objPoint.GIFTNAME.Contains("【汽車】"))
                                    {
                                        objPoint.GIFTNAME = "【汽車】\n" + objPoint.GIFTNAME;
                                    }
                                    TotalGiftPointCar += int.Parse(objPoint.GIFTPOINT);
                                    TotalLastPointCar += int.Parse(objPoint.LASTPOINT);
                                    TotalLastTransPointCar += (objPoint.AllowSend == 1 ? int.Parse(objPoint.LASTPOINT) : 0);
                                }
                                else if (objPoint.PointType == 1)
                                {
                                    if (!objPoint.GIFTNAME.Contains("【機車】"))
                                    {
                                        objPoint.GIFTNAME = "【機車】\n" + objPoint.GIFTNAME;
                                    }
                                    TotalGiftPointMotor += int.Parse(objPoint.GIFTPOINT);
                                    TotalLastPointMotor += int.Parse(objPoint.LASTPOINT);
                                    TotalLastTransPointMotor += (objPoint.AllowSend == 1 ? int.Parse(objPoint.LASTPOINT) : 0);
                                }

                                //點數加總
                                TotalGiftPoint += int.Parse(objPoint.GIFTPOINT);
                                TotalLastPoint += int.Parse(objPoint.LASTPOINT);
                            }
                        }

                        apiOutput.Bonus = new BonusForOrder()
                        {
                            TotalLASTPOINT = TotalLastPoint + OldCarPoint + OldMotorPoint, //回補已使用的汽機車點數
                            TotalCarLASTPOINT = TotalLastPointCar + OldCarPoint,         //汽車剩餘
                            TotalMotorLASTPOINT = TotalLastPointMotor + OldMotorPoint    //機車剩餘
                        };
                        SD = Convert.ToDateTime(apiOutput.OrderData.FS);
                        DateTime ED = Convert.ToDateTime(apiOutput.OrderData.FE);
                        List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), ED.ToString("yyyyMMdd"));
                        int days = 0, hours = 0, minutes = 0;
                        apiOutput.IsHoliday = (new BillCommon().IsInHoliday(lstHoliday, SD)) ? 1 : 0;
                        //移除逾時判斷
                        //if (apiOutput.OrderData.FT != "")
                        //{
                        //    ED = Convert.ToDateTime(apiOutput.OrderData.ED);
                        //}
                        new BillCommon().CalDayHourMin(SD, ED, ref days, ref hours, ref minutes);
                        //new BillCommon().GetTimePart(SD, ED, apiOutput.OrderData.PROJTYPE);
                        int needPointer = ((days * 60 * 10) + (hours * 60) + minutes);
                        if (apiOutput.OrderData.PROJTYPE == 4)
                        {
                            if (needPointer < apiOutput.OrderData.BaseMinutes)
                            {
                                needPointer = apiOutput.OrderData.BaseMinutes;
                            }
                            apiOutput.Bonus.CanUseTotalCarPoint = Math.Min(TotalLastPoint, needPointer);
                            apiOutput.Bonus.CanUseTotalMotorPoint = apiOutput.Bonus.CanUseTotalCarPoint;
                        }
                        else
                        {
                            if (apiOutput.OrderData.PROJTYPE < 4 && hours == 0)
                            //20210324 ADD BY ADAM REASON.補上日期判斷
                            if(apiOutput.OrderData.PROJTYPE<4 && hours == 0 && days ==0)
                            {
                                needPointer = 60;
                            }
                            apiOutput.Bonus.CanUseTotalMotorPoint = 0;
                            if ((needPointer % 30) >= 15)
                            {
                                needPointer += (30 - (needPointer % 30));
                            }
                            else
                            {
                                needPointer -= (needPointer % 30);
                            }
                            apiOutput.Bonus.CanUseTotalCarPoint = Math.Min(TotalLastPointCar, needPointer);

                        }

                    }
                }
                else
                {
                    //errCode = "ERR";
                    //errMsg = wsOutput.Message;
                }
            }

            // 20210427;增加LOG方便查問題
            logger.Trace(string.Format("OrderNo:{0} ApiOutput:{1}", tmpOrder, JsonConvert.SerializeObject(apiOutput)));
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}