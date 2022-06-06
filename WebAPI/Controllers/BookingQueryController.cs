using Domain.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.rootAPI;
using Newtonsoft.Json;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 訂單列表
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
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = -1;
            bool HasInput = false;

            var monSp = new MonSubsSp();
            BillCommon billCommon = new BillCommon();
            #endregion
            #region 防呆
            if (value != null)
                HasInput = true;

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, HasInput);

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_BookingQuery>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
            }

            if (flag)
            {
                if (apiInput != null)
                {
                    if (!string.IsNullOrWhiteSpace(apiInput.OrderNo))
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
            }

            //不開放訪客
            if (isGuest)
            {
                flag = false;
                errCode = "ERR101";
            }
            #endregion

            #region TB
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            //開始做取得訂單
            if (flag)
            {
                SPInput_GetOrderList spInput = new SPInput_GetOrderList()
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    OrderNo = tmpOrder,
                    LogID = LogID
                };
                string SPName = "usp_OrderListQuery";
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetOrderList, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetOrderList, SPOutput_Base>(connetStr);
                List<OrderQueryDataList> OrderDataLists = new List<OrderQueryDataList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref OrderDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    int DataLen = OrderDataLists.Count;
                    if (DataLen > 0)
                    {
                        outputApi = new OAPI_BookingQuery();
                        outputApi.OrderObj = new List<ActiveOrderData>();
                        for (int i = 0; i < DataLen; i++)
                        {
                            //20201026 ADD BY ADAM REASON.增加據點圖片
                            List<StationInfoObj> tmpStationInfoObj = JsonConvert.DeserializeObject<List<StationInfoObj>>(OrderDataLists[i].StationPicJson);
                            foreach (var StationInfo in tmpStationInfoObj)
                            {
                                StationInfo.StationPic = string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["StorageBaseURL"], ConfigurationManager.AppSettings["stationContainer"], StationInfo.StationPic);
                            }

                            ActiveOrderData obj = new ActiveOrderData()
                            {
                                CarNo = OrderDataLists[i].CarNo,
                                CarBrend = OrderDataLists[i].CarBrend,
                                CarOfArea = OrderDataLists[i].Area,
                                CarRentBill = OrderDataLists[i].init_price,
                                CarTypeName = OrderDataLists[i].CarTypeName,
                                CarTypePic = OrderDataLists[i].CarTypeImg,
                                Seat = OrderDataLists[i].Seat,
                                HolidayPerHour = OrderDataLists[i].PRICE_H,
                                Insurance = OrderDataLists[i].Insurance,
                                InsuranceBill = OrderDataLists[i].InsurancePurePrice,
                                InsurancePerHour = OrderDataLists[i].InsurancePerHours + OrderDataLists[i].JointInsurancePerHour,   // 20210831 UPD BY YEH REASON:安心服務每小時金額要加上共同承租人每小時安心服務價格
                                Operator = OrderDataLists[i].OperatorICon,
                                OperatorScore = OrderDataLists[i].Score,
                                OrderNo = string.Format("H{0}", OrderDataLists[i].order_number.ToString().PadLeft(7, '0')),
                                ParkingSection = OrderDataLists[i].parkingSpace,
                                IsMotor = OrderDataLists[i].IsMotor,
                                PickTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].final_start_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].final_start_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                ReturnTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].final_stop_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].final_stop_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                StartTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].start_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].start_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                StopTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].stop_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].stop_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                StopPickTime = (string.IsNullOrWhiteSpace(OrderDataLists[i].stop_pick_time)) ? "" : Convert.ToDateTime(OrderDataLists[i].stop_pick_time).ToString("yyyy-MM-dd HH:mm:ss"),
                                ProjName = OrderDataLists[i].PRONAME,
                                WorkdayPerHour = OrderDataLists[i].PRICE,
                                TransDiscount = (OrderDataLists[i].init_TransDiscount < 0) ? 0 : OrderDataLists[i].init_TransDiscount,
                                ProjType = OrderDataLists[i].ProjType,
                                MaxPrice = OrderDataLists[i].MaxPrice,
                                MaxPriceH = OrderDataLists[i].MaxPriceH,
                                StationInfo = new Domain.TB.iRentStationData()
                                {
                                    ADDR = OrderDataLists[i].ADDR,
                                    Content = OrderDataLists[i].Content,
                                    Latitude = OrderDataLists[i].Latitude,
                                    Longitude = OrderDataLists[i].Longitude,
                                    StationID = OrderDataLists[i].StationID,
                                    StationName = OrderDataLists[i].StationName,
                                    Tel = OrderDataLists[i].Tel,
                                    ContentForAPP = OrderDataLists[i].ContentForAPP,
                                    IsRequiredForReturn = OrderDataLists[i].IsRequiredForReturn,
                                    StationPic = tmpStationInfoObj
                                },
                                MileagePerKM = (OrderDataLists[i].ProjType < 4) ? ((OrderDataLists[i].MilageUnit == 0) ? Mildef : OrderDataLists[i].MilageUnit) : 0,
                                DailyMaxHour = 10,
                                CAR_MGT_STATUS = OrderDataLists[i].car_mgt_status,
                                AppStatus = OrderDataLists[i].AppStatus,     //20201026 ADD BY ADAM REASON.增加APP對應狀態
                                CarLatitude = OrderDataLists[i].CarLatitude,
                                CarLongitude = OrderDataLists[i].CarLongitude,
                                OpenDoorDeadLine = (string.IsNullOrWhiteSpace(OrderDataLists[i].OpenDoorDeadLine)) ? "" : Convert.ToDateTime(OrderDataLists[i].OpenDoorDeadLine).ToString("yyyy-MM-dd HH:mm:ss"),
                                RenterType = OrderDataLists[i].RenterType,   // 20210830 UPD BY YEH REASON:增加承租人類型
                                BaseInsuranceMinutes= OrderDataLists[i].BaseInsuranceMinutes,
                                BaseMotoRate= OrderDataLists[i].BaseMotoRate,
                                InsuranceMotoMin= OrderDataLists[i].InsuranceMotoMin,
                                InsuranceMotoRate= OrderDataLists[i].InsuranceMotoRate //20220531 UPD BY YANKEY REASON : 增加機車安心服務費用
                            };
                            obj.MileageBill = billCommon.CarMilageCompute(Convert.ToDateTime(obj.StartTime), Convert.ToDateTime(obj.StopTime), OrderDataLists[i].MilageUnit, Mildef, 20, new List<Holiday>());

                            if (obj.ProjType == 4)  //機車
                            {
                                obj.MotorBasePriceObj = new MotorBillBase()
                                {
                                    BaseMinutes = OrderDataLists[i].BaseMinutes,
                                    BasePrice = OrderDataLists[i].BaseMinutesPrice,
                                    MaxPrice = OrderDataLists[i].MaxPrice,
                                    PerMinutesPrice = OrderDataLists[i].MinuteOfPrice
                                };
                                obj.MotorPowerBaseObj = new MotorPowerInfoBase()
                                {
                                    //20210522 ADD BY ADAM REASON.如果可以讀到儀表板電量就以rsoc為主
                                    Power = OrderDataLists[i].deviceRSOC == "NA" || OrderDataLists[i].deviceRSOC == "" ? OrderDataLists[i].device3TBA : Convert.ToInt32(OrderDataLists[i].deviceRSOC),
                                    RemainingMileage = (OrderDataLists[i].RemainingMilage == "NA" || OrderDataLists[i].RemainingMilage == "") ? -1 : Convert.ToInt32(Convert.ToSingle(OrderDataLists[i].RemainingMilage))
                                };
                            }
                            // 前車圖片欄位 ADD 20200414 
                            obj.PreviousCarPath = (obj.ProjType == 4 ? (OrderDataLists[i].PreviousCarImage == "" ? "" : string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["StorageBaseURL"], ConfigurationManager.AppSettings["CarContainer"], OrderDataLists[i].PreviousCarImage)) : "");

                            if (!string.IsNullOrEmpty(OrderDataLists[i].LabelType) && OrderDataLists[i].GiveMinute > 0)
                            {
                                obj.DiscountLabel = new DiscountLabel()
                                {
                                    LabelType = OrderDataLists[i].LabelType,
                                    GiveMinute = OrderDataLists[i].GiveMinute,
                                    Describe = string.Format("{0}分鐘優惠折抵", OrderDataLists[i].GiveMinute)
                                };
                            }

                            obj.Bill = obj.CarRentBill + obj.InsuranceBill + obj.MileageBill - obj.TransDiscount;
                            obj.OrderStatus = GetOrderStatus(OrderDataLists[i].car_mgt_status, OrderDataLists[i].booking_status, OrderDataLists[i].already_lend_car, OrderDataLists[i].IsReturnCar, Convert.ToDateTime(obj.StopTime));
                            outputApi.OrderObj.Add(obj);
                        }
                    }
                }

                //置換訂閱制相關欄位
                if (flag)
                {
                    string sp_errCode = "";
                    var spIn = new SPInput_GetSubsMonthByOrderNo()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        OrderNo = tmpOrder
                    };
                    var subsMons = monSp.sp_GetSubsMonthByOrderNo(spIn, ref sp_errCode);
                    if (outputApi != null && outputApi.OrderObj != null && outputApi.OrderObj.Count() > 0)
                    {
                        if (subsMons != null && subsMons.Count() > 0)
                        {
                            outputApi.OrderObj.ForEach(x =>
                            {
                                Int64 xOrderNo = Convert.ToInt64(x.OrderNo.Replace("H", string.Empty));
                                var mlist = subsMons.Where(y => y.OrderNo == xOrderNo).ToList();
                                if (mlist != null && mlist.Count() > 0)
                                {
                                    var f = mlist.FirstOrDefault();
                                    x.ProjName = f.MonProjNM;
                                    if (x.ProjType == 4)
                                    {
                                        if (x.MotorBasePriceObj != null)
                                            x.MotorBasePriceObj.PerMinutesPrice = Convert.ToSingle(f.WDRateForMoto);//置換訂閱制價格
                                    }
                                    else
                                    {
                                        x.WorkdayPerHour = Convert.ToInt32(f.WDRateForCar);//置換訂閱制價格
                                        x.HolidayPerHour = Convert.ToInt32(f.HDRateForCar);//置換訂閱制價格
                                    }
                                }
                            });
                        }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        #region 取得訂單狀態
        private int GetOrderStatus(int car_mgt_status, int booking_status, int already_lend_car, int isReturn, DateTime? stop_time)
        {
            int OrderStatus = 0;
            if (car_mgt_status < 4)
            {
                if (already_lend_car == 0 && isReturn < 1)
                {
                    OrderStatus = -1;
                }
                else if (isReturn == 1)
                {
                    OrderStatus = 0;
                }
            }
            else if (car_mgt_status >= 4 && car_mgt_status < 11)
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
        #endregion
    }
}