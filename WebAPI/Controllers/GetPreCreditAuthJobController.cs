using Domain.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
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

        [HttpGet]
        public Dictionary<string, object> DoCreditAuthJob()
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
            
            IAPI_CreditAuth apiInput = null;
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
            
            /*以下 移到API INPUT*/
            int Nday = 2;
            int NHour = 6;
            int FirstReserveTime = 20;
            int AuthGateCount = 1;
            int ReservationAuthGateCount = 1;

            //List<OrderAuthList> OrderAuthList = null;
            #endregion
            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion
            #region TB
            #region 取出訂單資訊
            var PreOrderAuthList = new List<PreOrderAuth>();
            if (flag)
            {
                PreOrderAuthList = GetPreOrderAuthList( ref flag,ref lstError,ref errCode);
            }
            #endregion

            if (flag)
            {
                var bookingList = PreOrderAuthList.Where(p => p.AuthType == 1);
                var timeoutList = PreOrderAuthList.Where(p => p.AuthType == 5);

                foreach (var booking in bookingList)
                {
                    /*
                     --,start_time a_start_time, booking_date a_booking_date,stop_time a_stop_time
                     --,Convert(varchar(10),start_time,111) getCarDate,DateAdd(day,-2,DateAdd(hour,20,Convert(varchar(10),start_time,111))) BookingNDaysAgo
                     --,DateAdd(hour,-6,start_time) NowBookingbefore 
                     --,DateAdd(hour,6,dbo.GET_TWDATE()) 
                     --,DateAdd(hour,-2,dbo.GET_TWDATE())  TimeOver
                     --,datediff(hour,stop_time,dbo.GET_TWDATE()) OverTimes
                     
                     */

                    var insertFlag = false;

                    DateTime getCarDate = booking.start_time.Date;
                    DateTime BookingNDaysAgo = getCarDate.AddDays(-Nday);
                    DateTime ReserveBookingAuthTime = BookingNDaysAgo.AddHours(FirstReserveTime);
                    DateTime NowBookingbefore = booking.start_time.AddHours(-NHour);
                    
                    if(booking.booking_date > ReserveBookingAuthTime && booking.booking_date < NowBookingbefore) 
                    {
                        SPInput_OrderAuth input = new SPInput_OrderAuth
                        {
                            order_number = booking.order_number,
                            IDNO = booking.IDNO,
                            final_Price = booking.pre_final_Price,
                            CardType = 1,
                            AuthType = booking.AuthType,
                            GateNo = GetGateNo(AuthGateCount),
                            isRetry = 0,
                            AutoClose = 0,
                            PrgName = funName,
                            PrgUser = ""
                        };

                        insertFlag = commonService.InsertOrderAuth(input,ref errCode);
                    }
                    else
                    {
                        SPInput_OrderAuthReservation input = new SPInput_OrderAuthReservation
                        {
                            order_number = booking.order_number,
                            IDNO = booking.IDNO,
                            final_Price = booking.pre_final_Price,
                            CardType = 1,
                            AuthType = booking.AuthType,
                            GateNo = GetGateNo(ReservationAuthGateCount),
                            isRetry = 0,
                            AutoClose = 0,
                            PrgName = funName,
                            PrgUser = "",
                            AppointmentTime = ReserveBookingAuthTime
                        };

                        insertFlag = commonService.InsertOrderAuthReservation(input, ref errCode);
                    }
                }

                foreach (var timeoutObj in timeoutList)
                {
                    var insertFlag = false;
                    SPInput_OrderAuth input = new SPInput_OrderAuth
                    {
                        order_number = timeoutObj.order_number,
                        IDNO = timeoutObj.IDNO,
                        final_Price = 0,
                        CardType = 1,
                        AuthType = timeoutObj.AuthType,
                        GateNo = GetGateNo(AuthGateCount),
                        isRetry = 0,
                        AutoClose = 0,
                        PrgName = funName,
                        PrgUser = ""
                    };

                    insertFlag = commonService.InsertOrderAuth(input, ref errCode);
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        private List<PreOrderAuth> GetPreOrderAuthList(ref bool flag, ref List<ErrorInfo> lstError,ref string errCode)
        {
            var PreOrderAuthList = new List<PreOrderAuth>();

            SPInput_GetPreCreditAuthList spInput = new SPInput_GetPreCreditAuthList();
            //{
            //    //MINUTES = AuthResendMin
            //};

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




    }
}