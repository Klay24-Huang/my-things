using Domain.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetPreCreditAuthJobController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonFunc baseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoJob(Dictionary<string, object> value)
        {
            logger.Trace("Init");
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetPreCreditAuthJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            IAPI_GetPreCreditAuthJob apiInput = new IAPI_GetPreCreditAuthJob();
            NullOutput apiOutput = new NullOutput();
           
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CommonService commonService = new CommonService();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;
            
            int Nday = apiInput.Nday;
            int NHour =apiInput.NHour;
            int FirstReserveTime = apiInput.FirstReserveTime;
            int AuthGateCount = apiInput.AuthGateCount;
            int ReservationAuthGateCount = apiInput.ReservationAuthGateCount;
            int PrepaidDays = apiInput.PrepaidDays;

            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_GetPreCreditAuthJob>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                Nday = apiInput.Nday;
                NHour = apiInput.NHour;
                FirstReserveTime = apiInput.FirstReserveTime;
                AuthGateCount = apiInput.AuthGateCount;
                ReservationAuthGateCount = apiInput.ReservationAuthGateCount;
                PrepaidDays = apiInput.PrepaidDays;
            }
            #endregion
            #region TB
            #region 取出訂單資訊
            var PreOrderAuthList = new List<PreOrderAuth>();
            if (flag)
            {
                PreOrderAuthList = GetPreOrderAuthList(NHour, ref flag,ref lstError,ref errCode);
            }
            #endregion

            if (flag)
            {
                var bookingList = PreOrderAuthList.Where(p => p.AuthType == 1);
                var timeoutList = PreOrderAuthList.Where(p => p.AuthType == 5);

                //處理預約
                foreach (var bookingObj in bookingList)
                {
                    var insertFlag = false;

                    //取車日
                    DateTime getCarDate = bookingObj.start_time.Date;
                    //取天前N天
                    DateTime BookingNDaysAgo = getCarDate.AddDays(-Nday);
                    //第一次取授權的時間
                    DateTime ReserveBookingAuthTime = BookingNDaysAgo.AddHours(FirstReserveTime);
                    //在這時間點後預約的訂單 不走排程
                    DateTime NowBookingbefore = bookingObj.start_time.AddHours(-NHour);

                    //取車前N小時到取車日前X日的Y時預約的訂單 走即時授權的排程
                    if (bookingObj.booking_date > ReserveBookingAuthTime && bookingObj.booking_date < NowBookingbefore)
                    {
                        SPInput_OrderAuth input = SetOrderAuthObj(bookingObj, funName, AuthGateCount);

                        insertFlag = commonService.InsertOrderAuth(input, ref errCode, ref lstError);
                        if (!insertFlag)
                        {
                            string errString = SetErrorStringForLog("InsertOrderAuth", lstError, bookingObj);
                            logger.Trace(errString);
                            baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, errString);

                        }
                    }
                    else
                    {
                        //取車日前X日的Y時以前預約的訂單 走預約授權的排程
                        SPInput_OrderAuthReservation input = SetOrderAuthReservationObj(bookingObj, funName, ReservationAuthGateCount, ReserveBookingAuthTime);

                        insertFlag = commonService.InsertOrderAuthReservation(input, ref errCode, ref lstError);
                        if (!insertFlag)
                        {
                            string errString = SetErrorStringForLog("InsertOrderAuthReservation", lstError, bookingObj);
                            
                            logger.Trace(errString);
                            baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, errString);
                        }
                    }
                }

                // 20211216 UPD BY YEH REASON:副總說拿掉預授權
                //處理逾時
                //foreach (var timeoutObj in timeoutList)
                //{
                //    var insertFlag = false;

                //    timeoutObj.pre_final_Price = GetPrepaidAmount(timeoutObj.order_number, PrepaidDays, timeoutObj.stop_time);
                //    SPInput_OrderAuth input = SetOrderAuthObj(timeoutObj, funName, AuthGateCount);

                //    insertFlag = commonService.InsertOrderAuth(input, ref errCode,ref lstError);
                //    if (!insertFlag)
                //    {
                //        string errString = SetErrorStringForLog("InsertOrderAuth", lstError, timeoutObj);

                //        logger.Trace(errString);
                //        baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, errString);
                //    }
                //}

                errCode = "000000";
            }
            
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

        private List<PreOrderAuth> GetPreOrderAuthList(int NHour, ref bool flag, ref List<ErrorInfo> lstError,ref string errCode)
        {
            var PreOrderAuthList = new List<PreOrderAuth>();

            SPInput_GetPreCreditAuthList spInput = new SPInput_GetPreCreditAuthList
            {
                NHour = NHour
            };

            string SPName = "usp_GetPreCreditAuthList_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetPreCreditAuthList, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetPreCreditAuthList, SPOutput_Base>(connetStr);

            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref PreOrderAuthList, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (PreOrderAuthList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203";
                    logger.Trace("GetPreOrderAuthList Error:" + JsonConvert.SerializeObject(lstError));
                }
            }

            return PreOrderAuthList;
        }

        private int GetGateNo(int GateCount)
        {
            if (GateCount == 1)
                return 0;
            
            List<int> GateList = new List<int>();

            for(int i = 0;i < GateCount;i++)
            {
                GateList.Add(i);
            }

            return GateList.OrderBy(p=>Guid.NewGuid()).First();

        }

        private int GetPrepaidAmount(Int64 order_number,int prepaidDays,DateTime stop_time)
        {
            CommonService commonService = new CommonService();
            var orderInfo = commonService.GetOrderForPreAuth(order_number);

            var estimateData = new EstimateData
            {
                ProjID = orderInfo.ProjID,
                ProjType = orderInfo.ProjType,
                SD = stop_time,
                ED = stop_time.AddDays(prepaidDays),
                Insurance = orderInfo.Insurance,
                InsurancePerHours = orderInfo.InsurancePerHours,
                WeekdayPrice = orderInfo.WeekdayPrice,
                HoildayPrice = orderInfo.HoildayPrice
            };
            EstimateDetail outData ;
            commonService.EstimatePreAuthAmt(estimateData, out outData);
            return outData.estimateAmt;

        }

        private SPInput_OrderAuth SetOrderAuthObj (PreOrderAuth input,string funName,int GateCount)
        {
            SPInput_OrderAuth OrderAuthObj = new SPInput_OrderAuth
            {
                order_number = input.order_number,
                IDNO = input.IDNO,
                final_price = input.pre_final_Price,
                CardType = 1,
                AuthType = input.AuthType,
                GateNO = GetGateNo(GateCount),
                isRetry = 0,
                AutoClose = 0,
                PrgName = funName,
                PrgUser = ""
            };

            return OrderAuthObj;
        }

        private SPInput_OrderAuthReservation SetOrderAuthReservationObj(PreOrderAuth input, string funName, int GateCount,DateTime AppointmentTime)
        {
            SPInput_OrderAuthReservation OrderAuthObj = new SPInput_OrderAuthReservation
            {
                order_number = input.order_number,
                IDNO = input.IDNO,
                final_price = input.pre_final_Price,
                CardType = 1,
                AuthType = input.AuthType,
                GateNO = GetGateNo(GateCount),
                isRetry = 0,
                AutoClose = 0,
                PrgName = funName,
                PrgUser = "",
                AppointmentTime = AppointmentTime
            };

            return OrderAuthObj;
        }

        private string SetErrorStringForLog<T>(string title, List<ErrorInfo> lstError,T input)
        {
            string errString = $"{title} Error:{JsonConvert.SerializeObject(lstError)} InputObj:{JsonConvert.SerializeObject(input)}";
            
            return errString;
        }

    }
}