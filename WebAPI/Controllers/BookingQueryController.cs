using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 進行中的訂單查詢
    /// </summary>
    public class BookingQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
        [HttpPost]
        public Dictionary<string, object> DoBookingQuery(Dictionary<string, object> value)
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
            string funName = "BookingQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingQuery apiInput = null;
            OAPI_BookingQuery outputApi = null;
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest,false);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            else
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion
            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            //開始做取消預約
            if (flag)
            {
                SPInput_GetOrderList spInput = new SPInput_GetOrderList()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderList);
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetOrderList, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetOrderList, SPOutput_Base>(connetStr);
                List<OrderQueryDataList> OrderDataLists = new List<OrderQueryDataList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref OrderDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    BillCommon billCommon = new BillCommon();
                   
                    int DataLen = OrderDataLists.Count;
                    if (DataLen > 0)
                    {
                        outputApi = new OAPI_BookingQuery();
                        outputApi.OrderObj = new List<ActiveOrderData>();
                        for (int i = 0; i < DataLen; i++)
                        {
                            ActiveOrderData obj = new ActiveOrderData()
                            {

                                CarBrend = OrderDataLists[i].CarBrend,
                                CarOfArea = OrderDataLists[i].CarOfArea,
                                CarRentBill = OrderDataLists[i].init_price,
                                CarTypeName = OrderDataLists[i].CarTypeName,
                                CarTypePic = OrderDataLists[i].CarTypeImg,
                                HolidayPerHour = OrderDataLists[i].PRICE_H,
                                InsuranceBill = OrderDataLists[i].InsurancePurePrice,
                                InsurancePerHour = OrderDataLists[i].Insurance,
                                Operator = OrderDataLists[i].OperatorICon,
                                OperatorScore = OrderDataLists[i].Score,
                                OrderNo = string.Format("H{0}", OrderDataLists[i].order_number.ToString().PadLeft(7, '0')),
                                ParkingSection = OrderDataLists[i].parkingSpace,
                                PickTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].final_start_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].final_start_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                StartTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].start_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].start_time).ToString("yyyy-MM-dd HH:mm:ss") ,
                                StopTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].stop_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].stop_time).ToString("yyyy-MM-dd HH:mm:ss") ,
                                StopPickTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].stop_pick_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].stop_pick_time).ToString("yyyy-MM-dd HH:mm:ss") ,
                                ProjName = OrderDataLists[i].PRONAME,
                                WorkdayPerHour = OrderDataLists[i].PRICE,
                                TransDiscount = (OrderDataLists[i].init_TransDiscount < 0) ? 0 : OrderDataLists[i].init_TransDiscount,
                                ProjType = OrderDataLists[i].ProjType,
                                StationInfo = new Domain.TB.iRentStationData()
                                {
                                    ADDR = OrderDataLists[i].ADDR,
                                    Content = OrderDataLists[i].Content,
                                    Latitude = OrderDataLists[i].Latitude,
                                    Longitude = OrderDataLists[i].Longitude,
                                    StationID = OrderDataLists[i].StationID,
                                    StationName = OrderDataLists[i].StationName,
                                    Tel = OrderDataLists[i].Tel
                                },
                                MileagePerKM = (OrderDataLists[i].ProjType < 4) ? ((OrderDataLists[i].MilageUnit == 0) ? Mildef : OrderDataLists[i].MilageUnit) : 0,
                               


                            };
                            obj.MileageBill = billCommon.CalMilagePay(Convert.ToDateTime(obj.StartTime), Convert.ToDateTime(obj.StopTime), OrderDataLists[i].MilageUnit, Mildef, 20);

                            if (obj.ProjType == 4)
                            {
                                obj.MotorBasePriceObj = new Domain.TB.MotorBillBase()
                                {
                                    BaseMinutes = OrderDataLists[i].BaseMinutes,
                                    BasePrice = OrderDataLists[i].BaseMinutes,
                                    MaxPrice = OrderDataLists[i].MaxPrice,
                                    PerMinutesPrice = OrderDataLists[i].MinuteOfPrice
                                };
                                obj.MotorPowerBaseObj = new Domain.TB.MotorPowerInfoBase()
                                {
                                    Power = OrderDataLists[i].device3TBA,
                                    RemainingMileage = (OrderDataLists[i].RemainingMilage == "N") ? -1 : Convert.ToSingle(OrderDataLists[i].RemainingMilage)
                                };
                            }
                            
                            obj.Bill = obj.CarRentBill + obj.InsuranceBill + obj.MileageBill - obj.TransDiscount;
                            obj.OrderStatus = GetOrderStatus(OrderDataLists[i].car_mgt_status, OrderDataLists[i].booking_status, OrderDataLists[i].already_lend_car, OrderDataLists[i].IsReturnCar, Convert.ToDateTime(obj.StopTime));
                            outputApi.OrderObj.Add(obj);
                        }
                    }
                }

            }

            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
        private int GetOrderStatus(int car_mgt_status,int booking_status,int already_lend_car,int isReturn,DateTime? stop_time)
        {
            int OrderStatus = 0;
            if (car_mgt_status < 4)
            {
                if (already_lend_car == 0 && isReturn < 1)
                {
                    OrderStatus = -1;
                }else if (isReturn == 1)
                {
                    OrderStatus = 0;
                }
            }
            else if(car_mgt_status>=4 && car_mgt_status<11)
            {
                OrderStatus = 1;
                if (booking_status > 0)
                {
                    OrderStatus = 2;
                }
                if (stop_time.HasValue)
                {
                    if (stop_time.Value.Subtract(DateTime.Now).TotalMinutes < 30)
                    {
                        OrderStatus = 3;
                    }
                    if (DateTime.Now.Subtract(stop_time.Value).TotalMinutes > 0)
                    {
                        OrderStatus = 4;
                    }
                }
            }
            else
            {
                if (DateTime.Now.Subtract(stop_time.Value).TotalMinutes > 0)
                {
                    OrderStatus = 4;
                }
                else
                {
                    OrderStatus = 5;
                }
           
            }
        
            return OrderStatus;
        }
    }
}
