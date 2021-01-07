using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
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
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.ComboFunc;
using WebCommon;
using Domain.WebAPI.output.Mochi;
using WebAPI.Utils;
using System.Linq;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 重新計算租金明細(純租金，不折抵)
    /// </summary>
    public class RePayDetailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        private List<SPInput_CalFinalPrice> jsonDts { get; set; } 
        private List<RePayDetailErrVM> errList { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoRePayDetail(Dictionary<string, object> value)
        {
            #region 參數宣告
            jsonDts = new List<SPInput_CalFinalPrice>();
            errList = new List<RePayDetailErrVM>();
            string errCode = "";
            bool isGuest = true;
            bool flag = true;
            var baseVerify = new CommonFunc();
            HttpContext httpContext = HttpContext.Current;
            string funName = "RePayDetailController";
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            string Contentjson = "";
            var inApi = new IAPI_RePayDetailAll();
            var outApi = new OAPI_RePayDetailAll();
            var objOutput = new Dictionary<string, object>();    //輸出
            string errMsg = "ok";            
            #endregion

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (!string.IsNullOrWhiteSpace(Contentjson))
                inApi = JsonConvert.DeserializeObject<IAPI_RePayDetailAll>(Contentjson);

            var dts = GetRePayDetails(inApi.SD, inApi.ED, ref errCode, inApi.OrderNos);
           
            if (dts != null && dts.Count() > 0)
            {
                int disc = 0;
                int motoDisc = 0;
                if (dts.Count() == 1)
                {
                    disc = inApi.Discount;
                    motoDisc = inApi.MotorDiscount;
                }

                foreach(var item in dts)
                {
                    var input = new IAPI_RePayDetail()
                    {
                        OrderNo = item.OrderNo,
                        IDNO = item.IDNO,
                        IsSave = inApi.IsSave,
                        RePayMode = inApi.RePayMode,
                        Discount = disc,
                        MotorDiscount =motoDisc,
                        jsonOut = inApi.jsonOut
                    };
                    try
                    {
                        xDoRePayDetail(input);
                    }
                    catch(Exception ex)
                    {
                        var err = objUti.TTMap<RePayDetailVM, RePayDetailErrVM>(item);
                        err.errMsg = ex.Message;
                        errList.Add(err);
                    }
                }
            }

            if (errList != null && errList.Count() > 0)
            {
                errMsg = "含有錯誤資料";
                outApi.MsgData = errList;
                outApi.Result = false;                
            }

            if (inApi.jsonOut == 1 && jsonDts != null && jsonDts.Count() > 0)
                outApi.jsonOut = jsonDts;            

            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outApi, null);
            return objOutput;
        }

        private Dictionary<string, object> xDoRePayDetail(IAPI_RePayDetail apiInput)
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
            string funName = "xDoRePayDetail";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            //IAPI_RePayDetail apiInput = new IAPI_RePayDetail();
            //輸出沿用
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<OrderQueryFullData> OrderDataLists = null;
            int ProjType = 0;
            string Contentjson = "";
            bool isGuest = true;
            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            string IDNO = "";
            int Discount = 0; //要折抵的點數
            List<Holiday> lstHoliday = null; //假日列表
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime FineDate = new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
            int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;  //false:無月租;true:有月租
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
            CarRentInfo carInfo = new CarRentInfo();//車資料
            int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

            double nor_car_wDisc = 0;//只有一般時段時平日折扣
            double nor_car_hDisc = 0;//只有一般時段時價日折扣
            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

            int gift_point = 0;//使用時數(汽車)
            int gift_motor_point = 0;//使用時數(機車)
            int motoBaseMins = 6;//機車基本分鐘數
            int carBaseMins = 60;//汽車基本分鐘數
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(null, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            flag = true;
            errCode = "000000";
            Contentjson = JsonConvert.SerializeObject(apiInput);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_RePayDetail>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                var input = new IBIZ_InCheck()
                {
                    OrderNo = apiInput.OrderNo,
                    Discount = apiInput.Discount,
                    MotorDiscount = apiInput.MotorDiscount,
                    isGuest = isGuest,
                };
                var inck_re = cr_com.InCheck(input);
                if (inck_re != null)
                {
                    flag = inck_re.flag;
                    errCode = inck_re.errCode;
                    errMsg = inck_re.errMsg;
                    lstError = inck_re.lstError;
                    Discount = inck_re.Discount;
                    tmpOrder = inck_re.longOrderNo;
                }
            }

            #endregion
            #region 取出基本資料
            //Token判斷
            if (flag)
            {
                IDNO = apiInput.IDNO == null ? "" : apiInput.IDNO;

                #region 取出訂單資訊
                if (flag)
                {
                    SPInput_BE_GetOrderStatusByOrderNo spInput = new SPInput_BE_GetOrderStatusByOrderNo()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        LogID = LogID,
                        UserID = "99998"
                    };
                    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                    OrderDataLists = new List<OrderQueryFullData>();
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        OrderDataLists = objUti.ConvertToList<OrderQueryFullData>(ds.Tables[0]);
                    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                    //判斷訂單狀態
                    if (flag)
                    {
                        if (OrderDataLists.Count == 0)
                        {
                            flag = false;
                            errCode = "ERR203";
                        }
                    }
                }

                #endregion

            }
            #endregion

            #region 第二階段判斷及計價
            if (flag)
            {
                //取得專案狀態
                if (flag)
                {
                    ProjType = OrderDataLists[0].ProjType;
                    SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數
                    //機車路邊不計算預計還車時間
                    if (OrderDataLists[0].ProjType == 4)
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }
                    else
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }
                    FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                    FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (FED.Subtract(ED).Ticks > 0)
                    {
                        FineDate = ED;
                        hasFine = true;
                    }

                }

                #region 汽車計費資訊 
                //note:汽車計費資訊PayDetail
                int car_payAllMins = 0; //全部計費租用分鐘
                int car_payInMins = 0;//未超時計費分鐘
                int car_payOutMins = 0;//超時分鐘-顯示用
                double car_pay_in_wMins = 0;//未超時平日計費分鐘
                double car_pay_in_hMins = 0;//未超時假日計費分鐘
                double car_pay_out_wMins = 0;//超時平日計費分鐘
                double car_pay_out_hMins = 0;//超時假日計費分鐘
                int car_inPrice = 0;//未超時費用
                int car_outPrice = 0;//超時費用
                int car_n_price = OrderDataLists[0].PRICE;
                int car_h_price = OrderDataLists[0].PRICE_H;

                if (flag)
                {
                    if (ProjType != 4)
                    {
                        var input = new IBIZ_CRNoMonth()
                        {
                            car_n_price = car_n_price,
                            car_h_price = car_h_price,
                            WeekdayPrice = OrderDataLists[0].WeekdayPrice,
                            HoildayPrice = OrderDataLists[0].HoildayPrice,
                            SD = SD,
                            ED = ED,
                            FED = FED,
                            hasFine = hasFine,
                            carBaseMins = carBaseMins,
                            lstHoliday = lstHoliday
                        };
                        var car_re = cr_com.CRNoMonth(input);
                        if (car_re != null)
                        {
                            flag = car_re.flag;
                            car_payAllMins = car_re.car_payAllMins;
                            car_payInMins = car_re.car_payInMins;
                            car_payOutMins = car_re.car_payOutMins;
                            car_pay_in_wMins = car_re.car_pay_in_wMins;
                            car_pay_in_hMins = car_re.car_pay_in_hMins;
                            car_inPrice = car_re.car_inPrice;
                            car_outPrice = car_re.car_outPrice;
                        }
                    }
                }
                #endregion

                #region mark-查資料庫最後資料 
                //查資料庫最後資料
                //if (flag)
                //{
                //    SPInput_GetGIFTMINS spInput = new SPInput_GetGIFTMINS()
                //    {
                //        MEMIDNO = IDNO
                //    };
                //    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                //    SPOutput_GetGIFTMINS spOutBase = new SPOutput_GetGIFTMINS();
                //    SQLHelper<SPInput_GetGIFTMINS, SPOutput_GetGIFTMINS> sqlHelpQuery = new SQLHelper<SPInput_GetGIFTMINS, SPOutput_GetGIFTMINS>(connetStr);
                //    //OrderDataLists = new List<OrderQueryFullData>();
                //    DataSet ds = new DataSet();
                //    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                //    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                //    //判斷訂單狀態
                //    if (flag)
                //    {
                //        if (OrderDataLists.Count == 0)
                //        {
                //            flag = false;
                //            errCode = "ERR203";
                //        }
                //    }
                //}
                #endregion

                #region 與短租查時數
                if (flag && false)
                {
                    var inp = new IBIZ_NPR270Query()
                    {
                        IDNO = IDNO
                    };
                    var re270 = cr_com.NPR270Query(inp);
                    if (re270 != null)
                    {
                        flag = re270.flag;
                        MotorPoint = re270.MotorPoint;
                        CarPoint = re270.CarPoint;
                    }

                    //判斷輸入的點數有沒有超過總點數
                    if (ProjType == 4)
                    {
                        if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
                        {
                            //flag = false;
                            //errCode = "ERR205";
                        }
                        else
                        {
                            if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
                            {
                                flag = false;
                                errCode = "ERR207";
                            }
                        }

                        if (TotalRentMinutes <= 6 && Discount == 6)
                        {

                        }
                        else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
                        {
                            flag = false;
                            errCode = "ERR303";
                        }
                    }
                    else
                    {
                        if (Discount > 0 && Discount % 30 > 0)
                        {
                            flag = false;
                            errCode = "ERR206";
                        }
                        else
                        {
                            if (Discount > CarPoint)
                            {
                                flag = false;
                                errCode = "ERR207";
                            }
                        }
                    }
                }
                #endregion

                #region 建空模及塞入要輸出的值
                if (flag)
                {
                    outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
                    outputApi.DiscountAlertMsg = "";
                    outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                    outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
                    outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
                    outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
                    outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
                    outputApi.ProType = ProjType;
                    outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
                    {
                        BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
                        BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
                        CarNo = OrderDataLists[0].CarNo,
                        RedeemingTimeCarInterval = CarPoint.ToString(),
                        RedeemingTimeMotorInterval = MotorPoint.ToString(),
                        RedeemingTimeInterval = (ProjType == 4) ? (CarPoint + MotorPoint).ToString() : CarPoint.ToString(),
                        RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
                        RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                    };

                    if (ProjType == 4)
                    {
                        TotalPoint = (CarPoint + MotorPoint);
                        outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                        {
                            BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
                            BaseMinutes = OrderDataLists[0].BaseMinutes,
                            MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
                        };
                    }
                    else
                    {
                        TotalPoint = CarPoint;
                        outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
                        {
                            HoildayOfHourPrice = OrderDataLists[0].PRICE_H,
                            HourOfOneDay = 10,
                            WorkdayOfHourPrice = OrderDataLists[0].PRICE,
                            WorkdayPrice = OrderDataLists[0].PRICE * 10,
                            MilUnit = OrderDataLists[0].MilageUnit,
                            HoildayPrice = OrderDataLists[0].PRICE_H * 10
                        };
                    }
                    //20201201 ADD BY ADAM REASON.轉乘優惠
                    TransferPrice = OrderDataLists[0].init_TransDiscount;
                }
                #endregion

                #region 月租
                //note: 月租GetPayDetail
                if (flag && apiInput.RePayMode == 1)
                {
                    var item = OrderDataLists[0];
                    var motoDayMaxMins = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                    var input = new IBIZ_MonthRent()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        intOrderNO = tmpOrder,
                        ProjType = item.ProjType,
                        MotoDayMaxMins = motoDayMaxMins,
                        MinuteOfPrice = item.MinuteOfPrice,
                        hasFine = hasFine,
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        MotoBaseMins = motoBaseMins,
                        lstHoliday = lstHoliday,
                        Discount = Discount,
                        PRICE = item.PRICE,
                        PRICE_H = item.PRICE_H,
                        carBaseMins = 60
                    };
                    var mon_re = cr_com.MonthRentNoSave(input);
                    if (mon_re != null)
                    {
                        flag = mon_re.flag;
                        UseMonthMode = mon_re.UseMonthMode;
                        outputApi.IsMonthRent = mon_re.IsMonthRent;
                        carInfo = mon_re.carInfo;
                        Discount = mon_re.useDisc;
                        monthlyRentDatas = mon_re.monthlyRentDatas;

                        if(ProjType == 4)
                           outputApi.Rent.CarRental = mon_re.CarRental;//機車用
                        else
                           CarRentPrice += mon_re.CarRental;//汽車用
                    }
                }
                #endregion

                #region 開始計價
                if (flag)
                {
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (ProjType == 4)
                    {
                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                        }
                        else
                        {
                            var item = OrderDataLists[0];
                            var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                            carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, item.BaseMinutes, dayMaxMinns, null, null, Discount);
                            if (carInfo != null)
                                outputApi.Rent.CarRental = carInfo.RentInPay;
                        }

                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    }
                    else
                    {
                        #region 非月租折扣計算
                        var input = new IBIZ_CRNoMonthDisc()
                        {
                            UseMonthMode = UseMonthMode,
                            hasFine = hasFine,
                            SD = SD,
                            ED = ED,
                            FED = FED,
                            CarBaseMins = carBaseMins,
                            lstHoliday = lstHoliday,
                            car_n_price = car_n_price,
                            car_h_price = car_h_price,
                            Discount = apiInput.Discount
                        };
                        var disc_re = cr_com.CRNoMonthDisc(input);
                        if (disc_re != null)
                        {
                            nor_car_PayDisc = disc_re.nor_car_PayDisc;
                            nor_car_wDisc = disc_re.nor_car_wDisc;
                            nor_car_hDisc = disc_re.nor_car_hDisc;
                            nor_car_PayDiscPrice = disc_re.nor_car_PayDiscPrice;
                            Discount = disc_re.UseDisc;
                        }
                        #endregion

                        if (UseMonthMode)
                        {
                            outputApi.Rent.CarRental = CarRentPrice;
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
                        }
                        else
                        {
                            CarRentPrice = car_inPrice;//未逾時租用費用
                            if (hasFine)
                                outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                        }

                        if (Discount > 0)
                        {
                            double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
                            double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

                            if (UseMonthMode)
                            {

                            }
                            else
                            {
                                //非月租折扣
                                CarRentPrice -= nor_car_PayDiscPrice;
                                CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                            }
                        }
                        //安心服務
                        InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
                        if (InsurancePerHours > 0)
                        {
                            outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

                            //逾時安心服務計算
                            if (TotalFineRentMinutes > 0)
                            {
                                outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
                            }
                        }

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        //里程費計算修改，遇到取不到里程數的先以0元為主
                        //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                        if (OrderDataLists[0].start_mile == 0 ||
                            OrderDataLists[0].end_mile == 0 ||
                            ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) > 1000) ||
                            ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0)
                            )
                        {
                            outputApi.Rent.MileageRent = 0;
                        }
                        else
                        {
                            outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        }
                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    //20201202 ADD BY ADAM REASON.ETAG費用
                    outputApi.Rent.ETAGRental = etagPrice;

                    var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                    xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                    outputApi.Rent.TotalRental = xTotalRental;

                    #region 修正輸出欄位
                    //note: 修正輸出欄位PayDetail
                    if (ProjType == 4)
                    {
                        outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                        outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                        outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)
                        outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                        outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

                        var cDisc = apiInput.Discount;
                        var mDisc = apiInput.MotorDiscount;
                        if (carInfo.useDisc > 0)
                        {
                            int lastDisc = carInfo.useDisc;
                            var useMdisc = mDisc > carInfo.useDisc ? carInfo.useDisc : mDisc;
                            lastDisc -= useMdisc;
                            gift_motor_point = useMdisc;
                            if (lastDisc > 0)
                            {
                                var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
                                lastDisc -= useCdisc;
                                gift_point = useCdisc;
                            }
                        }
                    }
                    else
                    {
                        if (UseMonthMode)
                        {
                            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                            outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)
                            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                            outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
                            if (carInfo != null && carInfo.useDisc > 0)
                                gift_point = carInfo.useDisc;
                        }
                        else
                        {
                            outputApi.Rent.UseNorTimeInterval = Discount.ToString();
                            outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
                            outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_pay_in_wMins + car_pay_in_hMins).ToString();//可折抵租用時數
                            outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
                            gift_point = nor_car_PayDisc;
                        }

                        gift_motor_point = 0;
                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }

                    #endregion

                    //輸出json顯示
                    if(apiInput.jsonOut == 1)
                    {
                        if(jsonDts != null)
                        {
                            var jItem = new SPInput_CalFinalPrice()
                            {
                                IDNO = IDNO,
                                OrderNo = tmpOrder,
                                final_price = outputApi.Rent.TotalRental,
                                pure_price = outputApi.Rent.CarRental,
                                mileage_price = outputApi.Rent.MileageRent,
                                Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                                fine_price = outputApi.Rent.OvertimeRental,
                                gift_point = apiInput.Discount,
                                gift_motor_point = apiInput.MotorDiscount,
                                Etag = outputApi.Rent.ETAGRental,
                                parkingFee = outputApi.Rent.ParkingFee,
                                TransDiscount = outputApi.Rent.TransferPrice,
                                LogID = LogID
                            };
                            jsonDts.Add(jItem);
                        }
                    }

                    if(apiInput.IsSave == 1)
                    {
                        string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice_Re);
                        SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            final_price = outputApi.Rent.TotalRental,
                            pure_price = outputApi.Rent.CarRental,
                            mileage_price = outputApi.Rent.MileageRent,
                            Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                            fine_price = outputApi.Rent.OvertimeRental,
                            gift_point = apiInput.Discount,
                            gift_motor_point = apiInput.MotorDiscount,
                            Etag = outputApi.Rent.ETAGRental,
                            parkingFee = outputApi.Rent.ParkingFee,
                            TransDiscount = outputApi.Rent.TransferPrice,
                            LogID = LogID
                        };
                        SPOutput_Base SPOutput = new SPOutput_Base();
                        SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                    }
                }
                #endregion

            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                var errItem = new RePayDetailErrVM()
                {
                    OrderNo = apiInput.OrderNo,
                    IDNO = apiInput.IDNO,
                    errMsg = errCode
                };
                errList.Add(errItem);
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        private List<RePayDetailVM> GetRePayDetails(DateTime SD, DateTime ED, ref string errMsg, string OrderNos="")
        {
            List<RePayDetailVM> saveDetail = new List<RePayDetailVM>();

            string SPName = new ObjType().GetSPName(ObjType.SPType.GetRePayList);

            object[] param = new object[3];
            param[0] = SD;
            param[1] = ED;
            param[2] = OrderNos;

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";

            ds1 = WebApiClient.SPRetB(ServerInfo.GetServerInfo(), SPName, param, ref returnMessage, ref messageLevel, ref messageType);

            if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
            {
                if (ds1.Tables.Count > 0)
                {
                    saveDetail = objUti.ConvertToList<RePayDetailVM>(ds1.Tables[0]);
                }
            }
            else
                errMsg = returnMessage;

            return saveDetail;
        }

    }

    public class IAPI_RePayDetailAll
    {
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public string OrderNos { get; set; } = "";
        public int IsSave { get; set; } = 0;
        public int RePayMode { get; set; } = 0;
        /// <summary>
        /// 是否輸出json
        /// </summary>
        public int jsonOut { get; set; } = 0;

        //====以下單筆驗證才能使用
        /// <summary>
        /// 汽車時數
        /// </summary>
        public int Discount { set; get; } = 0;
        /// <summary>
        /// 機車時數
        /// </summary>
        public int MotorDiscount { set; get; } = 0;
    }

    public class OAPI_RePayDetailAll
    {
        public bool Result { get; set; } = true;
        public List<RePayDetailErrVM> MsgData { get; set; }
        public List<SPInput_CalFinalPrice> jsonOut { get; set; }
    }

    public class RePayDetailVM
    {
        public string OrderNo { get; set; }
        public string IDNO { get; set; }
        public DateTime final_start_time { get; set; }
        public DateTime final_stop_time { get; set; }
        public int final_price { get; set; }
        public int gift_point { get; set; }
        public int gift_motor_point { get; set; }
        public string RetCode { get; set; }
    }

    public class RePayDetailErrVM: RePayDetailVM
    {
        public string errMsg { get; set; }
    }

}