﻿using Domain.Common;
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
    /// 取得租金明細
    /// </summary>
    public class GetPayDetailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        [HttpPost]
        public Dictionary<string, object> DoGetPayDetail(Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var trace = new GetPayDetailTrace();
            var carRepo = new CarRentRepo(connetStr);
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetPayDetailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetPayDetail apiInput = null;
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
            DateTime? FineDate = null;
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
            int motoMaxMins = 200;//機車單日最大分鐘數
            int carBaseMins = 60;//汽車基本分鐘數

            #endregion
            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetPayDetail>(Contentjson);
                    trace.apiInput = apiInput;
                    trace.FlowList.Add("防呆");
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    trace.FlowList.Add("寫入API Log");
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
                        trace.InCheck = inck_re;
                        flag = inck_re.flag;
                        errCode = inck_re.errCode;
                        Discount = inck_re.Discount;
                        tmpOrder = inck_re.longOrderNo;
                    }
                    trace.FlowList.Add("input檢查");

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }
                }
                #endregion
                #region 取出基本資料
                //Token判斷
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
                        trace.TokenCk = token_re;
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                    trace.FlowList.Add("Token判斷");

                    #region 取出訂單資訊
                    if (flag)
                    {
                        SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            LogID = LogID,
                            Token = Access_Token
                        };
                        string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
                        SPOutput_Base spOutBase = new SPOutput_Base();
                        SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                        OrderDataLists = new List<OrderQueryFullData>();
                        DataSet ds = new DataSet();
                        flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            OrderDataLists = objUti.ConvertToList<OrderQueryFullData>(ds.Tables[0]);
                        baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                        trace.FlowList.Add("取出訂單資訊");
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

                    if (OrderDataLists != null && OrderDataLists.Count() > 0)
                    {
                        trace.OrderDataLists = OrderDataLists;
                        var item = OrderDataLists[0];
                        trace.OrderNo = item.OrderNo;
                        motoBaseMins = item.BaseMinutes > 0 ? item.BaseMinutes : motoBaseMins;
                        ProjType = item.ProjType;
                    }
                }
                #endregion

                #region 第二階段判斷及計價
                if (flag)
                {
                    //判斷狀態
                    if (OrderDataLists[0].car_mgt_status == 16 || OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
                    {
                        flag = false;
                        errCode = "ERR204";
                    }

                    //取得專案狀態
                    if (flag)
                    {
                        ProjType = OrderDataLists[0].ProjType;
                        SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                        SD = SD.AddSeconds(SD.Second * -1); //去秒數
                                                            //機車路邊不計算預計還車時間
                        if (!string.IsNullOrWhiteSpace(OrderDataLists[0].fine_Time))
                        {
                            FineDate = Convert.ToDateTime(OrderDataLists[0].fine_Time);
                            FineDate = FineDate.Value.AddSeconds(ED.Second * -1); //去秒數
                        }

                        var neverHasFine = new List<int>() { 3, 4 };//路邊機車不會逾時
                        if (neverHasFine.Contains(OrderDataLists[0].ProjType))
                        {
                            ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                            ED = ED.AddSeconds(ED.Second * -1); //去秒數
                            FineDate = null;
                        }
                        else
                        {
                            ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                            ED = ED.AddSeconds(ED.Second * -1); //去秒數
                        }
                        FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                        FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                        lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                          
                        if(FineDate != null)
                        {
                            if (FED > FineDate)
                            {
                                hasFine = true;
                                ED = FineDate.Value;
                            }
                        }
                        else
                        {
                            if (FED.Subtract(ED).Ticks > 0)
                                hasFine = true;
                        }
                        trace.hasFine = hasFine;
                        trace.FlowList.Add("SD,ED,FD計算");
                    }
                    if (flag)
                    {
                        if (NowTime.Subtract(FED).TotalMinutes >= 30)
                        {
                            flag = false;
                            errCode = "ERR208";
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
                                trace.CRNoMonth = car_re;
                                flag = car_re.flag;
                                car_payAllMins = car_re.car_payAllMins;
                                car_payInMins = car_re.car_payInMins;
                                car_payOutMins = car_re.car_payOutMins;
                                car_pay_in_wMins = car_re.car_pay_in_wMins;
                                car_pay_in_hMins = car_re.car_pay_in_hMins;
                                car_inPrice = car_re.car_inPrice;
                                car_outPrice = car_re.car_outPrice;
                            }
                            trace.FlowList.Add("汽車計費資訊(非月租)");
                        }
                    }

                    #endregion

                    #region 補outputApi
                    if (flag)
                    {
                        if (ProjType == 4)
                        {
                            var xre = billCommon.GetMotoRangeMins(SD, ED, motoBaseMins, motoMaxMins, new List<Holiday>());
                            if (xre != null)
                                TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1));
                        }
                        else
                            TotalRentMinutes = car_payAllMins;
                        trace.FlowList.Add("補outputApi");
                    }
                    #endregion

                    #region 與短租查時數
                    if (flag)
                    {
                        var inp = new IBIZ_NPR270Query()
                        {
                            IDNO = IDNO
                        };
                        var re270 = cr_com.NPR270Query(inp);
                        if (re270 != null)
                        {
                            trace.NPR270Query = re270;
                            flag = re270.flag;
                            MotorPoint = re270.MotorPoint;
                            CarPoint = re270.CarPoint;
                        }
                        trace.FlowList.Add("與短租查時數");

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
                            trace.FlowList.Add("機車一般點數檢查");
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
                            trace.FlowList.Add("汽車一般點數檢查");
                        }
                    }
                    #endregion
                    #region 查ETAG 20201202 ADD BY ADAM
                    if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要進來
                    {
                        var input = new IBIZ_ETagCk()
                        {
                            OrderNo = apiInput.OrderNo
                        };
                        var etag_re = cr_com.ETagCk(input);
                        if (etag_re != null)
                        {
                            trace.ETagCk = etag_re;
                            flag = etag_re.flag;
                            errCode = etag_re.errCode;
                            etagPrice = etag_re.etagPrice;
                        }
                        trace.FlowList.Add("查ETAG");
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
                        trace.FlowList.Add("建空模");
                    }
                    if (flag && OrderDataLists[0].ProjType != 4 && false) //20201224 add by adam 問題未確定前先關掉車麻吉
                    {
                        var input = new IBIZ_CarMagi()
                        {
                            LogID = LogID,
                            CarNo = OrderDataLists[0].CarNo,
                            SD = SD,
                            ED = FED.AddDays(1)
                        };
                        var magi_Re = cr_com.CarMagi(input);
                        if (magi_Re != null)
                        {
                            trace.CarMagi = magi_Re;
                            flag = magi_Re.flag;
                            outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                        }
                        trace.FlowList.Add("車麻吉");
                    }

                    #endregion
                    #region 月租
                    //note: 月租GetPayDetail
                    if (flag)
                    {
                        var item = OrderDataLists[0];
                        item = cr_com.dbValeFix(item);
                        var motoDayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);
                        var input = new IBIZ_MonthRent()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            intOrderNO = tmpOrder,
                            ProjType = item.ProjType,
                            MotoDayMaxMins = motoDayMaxMinns,
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
                        var mon_re = cr_com.MonthRentSave(input);
                        if (mon_re != null)
                        {
                            trace.MonthRent = mon_re;
                            flag = mon_re.flag;
                            UseMonthMode = mon_re.UseMonthMode;
                            outputApi.IsMonthRent = mon_re.IsMonthRent;
                            if (UseMonthMode)
                            {
                                carInfo = mon_re.carInfo;
                                Discount = mon_re.useDisc;
                                monthlyRentDatas = mon_re.monthlyRentDatas;

                                if (ProjType == 4)
                                    outputApi.Rent.CarRental = mon_re.CarRental;//機車用
                                else
                                    CarRentPrice += mon_re.CarRental;//汽車用
                            }
                        }
                        trace.FlowList.Add("月租");
                    }
                    #endregion
                    #region 開始計價
                    if (flag)
                    {
                        trace.FlowList.Add("開始計價");
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

                                carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, null, null, Discount);
                                if (carInfo != null)
                                    outputApi.Rent.CarRental = carInfo.RentInPay;
                                trace.FlowList.Add("機車非月租租金計算");
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
                                trace.CRNoMonthDisc = disc_re;
                                nor_car_PayDisc = disc_re.nor_car_PayDisc;
                                nor_car_wDisc = disc_re.nor_car_wDisc;
                                nor_car_hDisc = disc_re.nor_car_hDisc;
                                nor_car_PayDiscPrice = disc_re.nor_car_PayDiscPrice;
                                Discount = disc_re.UseDisc;
                            }
                            trace.FlowList.Add("非月租折扣計算");
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
                                trace.FlowList.Add("汽車非月租金額給值");
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
                                    trace.FlowList.Add("汽車非月租折扣扣除");
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
                            trace.FlowList.Add("安心服務");

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
                            trace.FlowList.Add("里程費計算");
                        }

                        outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                        outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                        outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                        //20201202 ADD BY ADAM REASON.ETAG費用
                        outputApi.Rent.ETAGRental = etagPrice;

                        var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                        xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                        outputApi.Rent.TotalRental = xTotalRental;
                        trace.FlowList.Add("總價計算");

                        #region 修正輸出欄位
                        //note: 修正輸出欄位PayDetail
                        if (ProjType == 4)
                        {
                            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                            outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

                            //2020-12-29 所有點數改成皆可折抵
                            //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

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

                                //2020 - 12 - 29 所有點數改成皆可折抵
                                //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                                outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

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
                        trace.FlowList.Add("修正輸出欄位");
                        #endregion

                        string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice);
                        SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            final_price = outputApi.Rent.TotalRental,
                            pure_price = outputApi.Rent.CarRental,
                            mileage_price = outputApi.Rent.MileageRent,
                            Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                            fine_price = outputApi.Rent.OvertimeRental,
                            gift_point = gift_point,
                            gift_motor_point = gift_motor_point,
                            monthly_workday = carInfo.useMonthDiscW,
                            monthly_holiday = carInfo.useMonthDiscH,
                            Etag = outputApi.Rent.ETAGRental,
                            parkingFee = outputApi.Rent.ParkingFee,
                            TransDiscount = outputApi.Rent.TransferPrice,
                            Token = Access_Token,
                            LogID = LogID,
                        };

                        #region trace
                        trace.TotalRentMinutes = TotalRentMinutes;
                        trace.Discount = Discount;
                        trace.CarPoint = CarPoint;
                        trace.MotorPoint = MotorPoint;
                        trace.SPInput = SPInput;
                        trace.outputApi = outputApi;
                        trace.carInfo = carInfo;
                        #endregion

                        SPOutput_Base SPOutput = new SPOutput_Base();
                        SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                        trace.FlowList.Add("sp存檔");
                    }
                    #endregion
                }
                #endregion

                #region 寫入錯誤Log
                if (!flag || logicCk(trace))
                {
                    trace.errCode = errCode;
                    trace.TotalPoint = TotalPoint;
                    trace.TransferPrice = TransferPrice;
                    string traceMsg = JsonConvert.SerializeObject(trace);
                    var errItem = new TraceLogVM()
                    {
                        ApiId = 73,
                        ApiMsg = traceMsg,
                        ApiNm = funName,
                        CodeVersion = trace.codeVersion,
                        FlowStep = trace.FlowStep(),
                        OrderNo = trace.OrderNo,
                        TraceType = eumTraceType.followErr
                    };
                    carRepo.AddTraceLog(errItem);
                }
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
            catch (Exception ex)
            {
                string exMsg = JsonConvert.SerializeObject(value) + ex.Message;
                var errItem = new TraceLogVM()
                {
                    ApiId = 73,
                    ApiMsg = exMsg,
                    ApiNm = funName,
                    CodeVersion = trace.codeVersion,
                    FlowStep = JsonConvert.SerializeObject(trace),
                    OrderNo = trace.OrderNo,
                    TraceType = eumTraceType.exception
                };
                carRepo.AddTraceLog(errItem);
                throw;
            }
        }

        private bool logicCk(GetPayDetailTrace sour)
        {
            if (sour != null)
            {
                if (sour.SPInput != null && sour.SPInput.pure_price == 0)
                    return true;
                else if (sour.hasFine)
                    return true;
                //有使用月租加入trace
                else if (sour.MonthRent != null && sour.MonthRent.monthlyRentDatas != null
                    && sour.MonthRent.monthlyRentDatas.Count() > 0 &&
                    sour.MonthRent.carInfo != null &&
                    sour.MonthRent.carInfo.mFinal != null &&
                    sour.MonthRent.carInfo.mFinal.Any(x => x.WorkDayHours > 0 ||
                    x.HolidayHours > 0 || x.MotoTotalHours >0)
                    )
                    return true;
            }
                
            return false;
        }

        #region mark
        //[HttpPost]
        //public Dictionary<string, object> DoGetPayDetail(Dictionary<string, object> value)
        //{
        //    #region 初始宣告
        //    HttpContext httpContext = HttpContext.Current;
        //    //string[] headers=httpContext.Request.Headers.AllKeys;
        //    string Access_Token = "";
        //    string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
        //    var objOutput = new Dictionary<string, object>();    //輸出
        //    bool flag = true;
        //    bool isWriteError = false;
        //    string errMsg = "Success"; //預設成功
        //    string errCode = "000000"; //預設成功
        //    string funName = "GetPayDetailController";
        //    Int64 LogID = 0;
        //    Int16 ErrType = 0;
        //    IAPI_GetPayDetail apiInput = null;
        //    OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
        //    Int64 tmpOrder = -1;
        //    Token token = null;
        //    CommonFunc baseVerify = new CommonFunc();
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();
        //    List<OrderQueryFullData> OrderDataLists = null;
        //    int ProjType = 0;
        //    string Contentjson = "";
        //    bool isGuest = true;
        //    int TotalPoint = 0; //總點數
        //    int MotorPoint = 0; //機車點數
        //    int CarPoint = 0;   //汽車點數
        //    string IDNO = "";
        //    int Discount = 0; //要折抵的點數
        //    List<Holiday> lstHoliday = null; //假日列表
        //    DateTime SD = new DateTime();
        //    DateTime ED = new DateTime();
        //    DateTime FED = new DateTime();
        //    DateTime FineDate = new DateTime();
        //    bool hasFine = false; //是否逾時
        //    DateTime NowTime = DateTime.Now;
        //    int TotalRentMinutes = 0; //總租車時數
        //    int TotalFineRentMinutes = 0; //總逾時時數
        //    int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
        //    int days = 0; int hours = 0; int mins = 0; //以分計費總時數
        //    int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
        //    int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
        //    int ActualRedeemableTimePoint = 0; //實際可抵折點數
        //    int CarRentPrice = 0; //車輛租金
        //    int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
        //    int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
        //    int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
        //    MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
        //    BillCommon billCommon = new BillCommon();
        //    List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
        //    bool UseMonthMode = false;  //false:無月租;true:有月租
        //    int InsurancePerHours = 0;  //安心服務每小時價
        //    int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
        //    CarRentInfo carInfo = new CarRentInfo();//車資料
        //    int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

        //    double nor_car_wDisc = 0;//只有一般時段時平日折扣
        //    double nor_car_hDisc = 0;//只有一般時段時價日折扣
        //    int nor_car_PayDisc = 0;//只有一般時段時總折扣
        //    int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

        //    int gift_point = 0;//使用時數(汽車)
        //    int gift_motor_point = 0;//使用時數(機車)
        //    int motoBaseMins = 6;//機車基本分鐘數
        //    int carBaseMins = 60;//汽車基本分鐘數

        //    #endregion

        //    #region 防呆
        //    flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

        //    if (flag)
        //    {
        //        apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetPayDetail>(Contentjson);
        //        //寫入API Log
        //        string ClientIP = baseVerify.GetClientIp(Request);
        //        flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

        //        if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
        //        {
        //            flag = false;
        //            errCode = "ERR900";
        //        }
        //        else
        //        {
        //            if (apiInput.OrderNo.IndexOf("H") < 0)
        //            {
        //                flag = false;
        //                errCode = "ERR900";
        //            }
        //            if (flag)
        //            {
        //                flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
        //                if (flag)
        //                {
        //                    if (tmpOrder <= 0)
        //                    {
        //                        flag = false;
        //                        errCode = "ERR900";
        //                    }
        //                }
        //            }
        //        }
        //        if (flag)
        //        {
        //            if (apiInput.Discount < 0)
        //            {
        //                flag = false;
        //                errCode = "ERR202";
        //            }

        //            if (apiInput.MotorDiscount < 0)
        //            {
        //                flag = false;
        //                errCode = "ERR202";
        //            }

        //            Discount = apiInput.Discount + apiInput.MotorDiscount;
        //        }

        //        //不開放訪客
        //        if (flag)
        //        {
        //            if (isGuest)
        //            {
        //                flag = false;
        //                errCode = "ERR101";
        //            }
        //        }
        //    }
        //    #endregion
        //    #region 取出基本資料
        //    //Token判斷
        //    if (flag && isGuest == false)
        //    {
        //        string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
        //        SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
        //        {
        //            LogID = LogID,
        //            Token = Access_Token
        //        };
        //        SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
        //        SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
        //        flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
        //        baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
        //        if (flag)
        //        {
        //            IDNO = spOut.IDNO;
        //        }

        //        #region 取出訂單資訊
        //        if (flag)
        //        {
        //            SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
        //            {
        //                IDNO = IDNO,
        //                OrderNo = tmpOrder,
        //                LogID = LogID,
        //                Token = Access_Token
        //            };
        //            string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
        //            SPOutput_Base spOutBase = new SPOutput_Base();
        //            SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
        //            OrderDataLists = new List<OrderQueryFullData>();
        //            DataSet ds = new DataSet();
        //            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
        //            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
        //            //判斷訂單狀態
        //            if (flag)
        //            {
        //                if (OrderDataLists.Count == 0)
        //                {
        //                    flag = false;
        //                    errCode = "ERR203";
        //                }


        //            }
        //        }

        //        #endregion

        //        if (OrderDataLists != null && OrderDataLists.Count() > 0)
        //            motoBaseMins = OrderDataLists[0].BaseMinutes > 0 ? OrderDataLists[0].BaseMinutes : motoBaseMins;
        //    }
        //    #endregion

        //    #region 第二階段判斷及計價
        //    if (flag)
        //    {
        //        //判斷狀態
        //        if (OrderDataLists[0].car_mgt_status == 16 || OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
        //        {
        //            flag = false;
        //            errCode = "ERR204";
        //        }

        //        //取得專案狀態
        //        if (flag)
        //        {
        //            ProjType = OrderDataLists[0].ProjType;
        //            SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
        //            SD = SD.AddSeconds(SD.Second * -1); //去秒數
        //            //機車路邊不計算預計還車時間
        //            if (OrderDataLists[0].ProjType == 4)
        //            {
        //                ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
        //                ED = ED.AddSeconds(ED.Second * -1); //去秒數
        //            }
        //            else
        //            {
        //                ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
        //                ED = ED.AddSeconds(ED.Second * -1); //去秒數
        //            }
        //            FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
        //            FED = FED.AddSeconds(FED.Second * -1);  //去秒數
        //            lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
        //            if (FED.Subtract(ED).Ticks > 0)
        //            {
        //                FineDate = ED;
        //                hasFine = true;
        //                billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
        //                TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
        //                billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
        //                TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數
        //                TotalFineInsuranceMinutes = ((FineDays * 6) + FineHours) * 60 + FineMins;  //逾時的安心服務總計(一日上限6小時)
        //            }
        //            else
        //            {
        //                billCommon.CalDayHourMin(SD, FED, ref days, ref hours, ref mins); //未逾時的總時數
        //                TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
        //            }
        //        }
        //        if (flag)
        //        {
        //            if (NowTime.Subtract(FED).TotalMinutes >= 30)
        //            {
        //                flag = false;
        //                errCode = "ERR208";
        //            }
        //        }

        //        #region 汽車計費資訊 
        //        //note:汽車計費資訊PayDetail
        //        int car_payAllMins = 0; //全部計費租用分鐘
        //        int car_payInMins = 0;//未超時計費分鐘
        //        int car_payOutMins = 0;//超時分鐘-顯示用

        //        double car_pay_in_wMins = 0;//未超時平日計費分鐘
        //        double car_pay_in_hMins = 0;//未超時假日計費分鐘
        //        double car_pay_out_wMins = 0;//超時平日計費分鐘
        //        double car_pay_out_hMins = 0;//超時假日計費分鐘

        //        int car_inPrice = 0;//未超時費用
        //        int car_outPrice = 0;//超時費用

        //        int car_n_price = OrderDataLists[0].PRICE;
        //        int car_h_price = OrderDataLists[0].PRICE_H;

        //        if (flag)
        //        {
        //            if (ProjType == 4)
        //            {

        //            }
        //            else
        //            {
        //                if (hasFine)
        //                {
        //                    var reInMins = billCommon.GetCarRangeMins(SD, ED, carBaseMins, 600, lstHoliday);
        //                    if (reInMins != null)
        //                    {
        //                        car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
        //                        car_payAllMins += car_payInMins;
        //                        car_pay_in_wMins = reInMins.Item1;
        //                        car_pay_in_hMins = reInMins.Item2;
        //                    }

        //                    var reOutMins = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
        //                    if (reOutMins != null)
        //                    {
        //                        car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
        //                        car_payAllMins += car_payOutMins;
        //                        car_pay_out_wMins = reOutMins.Item1;
        //                        car_pay_out_hMins = reOutMins.Item2;
        //                    }

        //                    car_inPrice = billCommon.CarRentCompute(SD, ED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
        //                    car_outPrice = billCommon.CarRentCompute(ED, FED, OrderDataLists[0].WeekdayPrice, OrderDataLists[0].HoildayPrice, 6, lstHoliday, true, 0);
        //                }
        //                else
        //                {
        //                    var reAllMins = billCommon.GetCarRangeMins(SD, FED, carBaseMins, 600, lstHoliday);
        //                    if (reAllMins != null)
        //                    {
        //                        car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
        //                        car_payInMins = car_payAllMins;
        //                        car_pay_in_wMins = reAllMins.Item1;
        //                        car_pay_in_hMins = reAllMins.Item2;
        //                    }

        //                    car_inPrice = billCommon.CarRentCompute(SD, FED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
        //                }
        //            }
        //        }

        //        #endregion

        //        #region 與短租查時數
        //        if (flag)
        //        {
        //            WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
        //            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
        //            flag = wsAPI.NPR270Query(IDNO, ref wsOutput);
        //            if (flag)
        //            {
        //                int giftLen = wsOutput.Data.Length;

        //                if (giftLen > 0)
        //                {
        //                    for (int i = 0; i < giftLen; i++)
        //                    {
        //                        DateTime tmpDate;
        //                        int tmpPoint = 0;
        //                        bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
        //                        bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
        //                        if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
        //                        {
        //                            if (wsOutput.Data[i].GIFTTYPE == "01")  //汽車
        //                            {
        //                                CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
        //                            }
        //                            else
        //                            {
        //                                MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            flag = true;
        //            errCode = "000000";

        //            //判斷輸入的點數有沒有超過總點數
        //            if (ProjType == 4)
        //            {
        //                if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
        //                {
        //                    //flag = false;
        //                    //errCode = "ERR205";
        //                }
        //                else
        //                {
        //                    if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
        //                    {
        //                        flag = false;
        //                        errCode = "ERR207";
        //                    }
        //                }

        //                if (TotalRentMinutes <= 6 && Discount == 6)
        //                {

        //                }
        //                else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
        //                {
        //                    flag = false;
        //                    errCode = "ERR303";
        //                }

        //                if (flag)
        //                {
        //                    billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
        //                }
        //            }
        //            else
        //            {
        //                if (Discount > 0 && Discount % 30 > 0)
        //                {
        //                    flag = false;
        //                    errCode = "ERR206";
        //                }
        //                else
        //                {
        //                    if (Discount > CarPoint)
        //                    {
        //                        flag = false;
        //                        errCode = "ERR207";
        //                    }
        //                }
        //                if (flag)
        //                {
        //                    billCommon.CalPointerToDayHourMin(CarPoint, ref PDays, ref PHours, ref PMins);
        //                }
        //            }
        //        }
        //        #endregion
        //        #region 查ETAG 20201202 ADD BY ADAM
        //        if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要進來
        //        {
        //            WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
        //            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
        //            //ETAG查詢失敗也不影響流程
        //            flag = wsAPI.ETAG010Send(apiInput.OrderNo, "", ref wsOutput);
        //            if (flag)
        //            {
        //                if (wsOutput.RtnCode == "0")
        //                {
        //                    //取出ETAG費用
        //                    if (wsOutput.Data.Length > 0)
        //                    {
        //                        etagPrice = wsOutput.Data[0].TAMT == "" ? 0 : int.Parse(wsOutput.Data[0].TAMT);

        //                    }
        //                }
        //            }
        //            flag = true;
        //            errCode = "000000";
        //        }
        //        #endregion


        //        #region 建空模及塞入要輸出的值
        //        if (flag)
        //        {
        //            outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
        //            outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
        //            outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
        //            outputApi.DiscountAlertMsg = "";
        //            outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
        //            outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
        //            outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
        //            outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
        //            outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
        //            outputApi.ProType = ProjType;
        //            outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
        //            {
        //                BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
        //                BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
        //                CarNo = OrderDataLists[0].CarNo,
        //                RedeemingTimeCarInterval = CarPoint.ToString(),
        //                RedeemingTimeMotorInterval = MotorPoint.ToString(),
        //                RedeemingTimeInterval = (ProjType == 4) ? (CarPoint + MotorPoint).ToString() : CarPoint.ToString(),
        //                RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
        //                RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
        //            };

        //            if (ProjType == 4)
        //            {
        //                TotalPoint = (CarPoint + MotorPoint);
        //                outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
        //                {
        //                    BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
        //                    BaseMinutes = OrderDataLists[0].BaseMinutes,
        //                    MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
        //                };
        //            }
        //            else
        //            {
        //                TotalPoint = CarPoint;
        //                outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
        //                {
        //                    HoildayOfHourPrice = OrderDataLists[0].PRICE_H,
        //                    HourOfOneDay = 10,
        //                    WorkdayOfHourPrice = OrderDataLists[0].PRICE,
        //                    WorkdayPrice = OrderDataLists[0].PRICE * 10,
        //                    MilUnit = OrderDataLists[0].MilageUnit,
        //                    HoildayPrice = OrderDataLists[0].PRICE_H * 10
        //                };
        //            }
        //            //20201201 ADD BY ADAM REASON.轉乘優惠
        //            TransferPrice = OrderDataLists[0].init_TransDiscount;


        //        }
        //        if (flag && OrderDataLists[0].ProjType != 4 && false) //20201224 add by adam 問題未確定前先關掉車麻吉
        //        {
        //            //檢查有無車麻吉停車費用
        //            WebAPIOutput_QueryBillByCar mochiOutput = new WebAPIOutput_QueryBillByCar();
        //            MachiComm mochi = new MachiComm();
        //            flag = mochi.GetParkingBill(LogID, OrderDataLists[0].CarNo, SD, FED.AddDays(1), ref ParkingPrice, ref mochiOutput);
        //            if (flag)
        //            {
        //                outputApi.Rent.ParkingFee = ParkingPrice;
        //            }
        //        }

        //        #endregion
        //        #region 月租
        //        //note: 月租GetPayDetail
        //        if (flag)
        //        {
        //            //1.0 先還原這個單號使用的
        //            flag = monthlyRentRepository.RestoreHistory(IDNO, tmpOrder, LogID, ref errCode);

        //            int RateType = (ProjType == 4) ? 1 : 0;
        //            if (!hasFine)
        //            {
        //                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
        //            }
        //            else
        //            {
        //                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
        //            }
        //            int MonthlyLen = monthlyRentDatas.Count;
        //            //先計算剩餘時數，以免落入基消陷阱
        //            //int MonthAll = 0;
        //            //for (int i=0;i< MonthlyLen;i++)
        //            //{
        //            //    MonthAll += Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);
        //            //}
        //            //先設定一遍
        //            //outputApi.Rent.UseMonthlyTimeInterval = MonthlyPoint.ToString();
        //            if (MonthlyLen > 0)
        //            {
        //                UseMonthMode = true;
        //                outputApi.IsMonthRent = 1;

        //                if (flag)
        //                {
        //                    if (ProjType == 4)
        //                    {
        //                        var motoMonth = objUti.Clone(monthlyRentDatas);
        //                        var item = OrderDataLists[0];
        //                        var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

        //                        int motoDisc = Discount;
        //                        carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, lstHoliday, motoMonth, motoDisc);

        //                        if (carInfo != null)
        //                        {
        //                            outputApi.Rent.CarRental += carInfo.RentInPay;
        //                            if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                                motoMonth = carInfo.mFinal;
        //                            Discount = carInfo.useDisc;
        //                        }

        //                        motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
        //                        if (motoMonth.Count > 0)
        //                        {
        //                            UseMonthMode = true;
        //                            int UseLen = motoMonth.Count;
        //                            for (int i = 0; i < UseLen; i++)
        //                            {
        //                                flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), LogID, ref errCode); //寫入記錄
        //                            }
        //                        }
        //                        else
        //                        {
        //                            UseMonthMode = false;
        //                        }

        //                        #region mark

        //                        //if (MonthAll > 0)
        //                        //{
        //                        //    //機車沒有分平假日，直接送即可
        //                        //    for (int i = 0; i < MonthlyLen; i++)
        //                        //    {
        //                        //        int MotoTotalMinutes = Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);     //換算分鐘數
        //                        //        if (MotoTotalMinutes >= TotalRentMinutes && TotalRentMinutes >= 6) //全部扣光
        //                        //        {
        //                        //            MonthlyPoint += TotalRentMinutes;    //20201128 ADD BY ADAM REASON.月租折抵點數計算
        //                        //            flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, TotalRentMinutes, LogID, ref errCode);//寫入記錄
        //                        //            TotalRentMinutes = 0;

        //                        //        }
        //                        //        else
        //                        //        {
        //                        //            //折抵不能全折時，基本分鐘數會擺在最後折，且要一次折抵掉
        //                        //            //一般時數會先折抵基本分鐘數，所以月租必須先折非基本分鐘，否則兩邊會有牴觸
        //                        //            if (TotalRentMinutes >= 6)
        //                        //            {
        //                        //                if ((TotalRentMinutes - MotoTotalMinutes) >= OrderDataLists[0].BaseMinutes) //扣完有超過基本費
        //                        //                {
        //                        //                    MonthlyPoint += MotoTotalMinutes;        //20201128 ADD BY ADAM REASON.月租折抵點數計算
        //                        //                    TotalRentMinutes -= MotoTotalMinutes;
        //                        //                    flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, MotoTotalMinutes, LogID, ref errCode); //寫入記錄
        //                        //                }
        //                        //                else
        //                        //                {
        //                        //                    //折抵時數不夠扣基本費 只能折  租用時數-基本分鐘數
        //                        //                    int tmpMonthlyPoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
        //                        //                    MonthlyPoint += tmpMonthlyPoint;
        //                        //                    TotalRentMinutes -= tmpMonthlyPoint;
        //                        //                    //MotoTotalMinutes += TotalRentMinutes - MotoTotalMinutes - OrderDataLists[0].BaseMinutes;
        //                        //                    //TotalRentMinutes -= MotoTotalMinutes;
        //                        //                    flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, tmpMonthlyPoint, LogID, ref errCode); //寫入記錄
        //                        //                }
        //                        //            }
        //                        //        }

        //                        //        outputApi.Rent.UseMonthlyTimeInterval = MonthlyPoint.ToString();
        //                        //    }
        //                        //}

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

        //                        UseMonthlyRent = monthlyRentDatas;

        //                        int xDiscount = Discount;//帶入月租運算的折扣
        //                        if (hasFine)
        //                        {
        //                            //CarRentPrice = billCommon.CalBillBySubScription(SD, ED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
        //                            carInfo = billCommon.CarRentInCompute(SD, ED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
        //                            if (carInfo != null)
        //                            {
        //                                CarRentPrice += carInfo.RentInPay;
        //                                if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                                    UseMonthlyRent = carInfo.mFinal;
        //                                Discount = carInfo.useDisc;
        //                            }
        //                            CarRentPrice += car_outPrice;
        //                        }
        //                        else
        //                        {
        //                            //CarRentPrice = billCommon.CalBillBySubScription(SD, FED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
        //                            carInfo = billCommon.CarRentInCompute(SD, FED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
        //                            if (carInfo != null)
        //                            {
        //                                CarRentPrice += carInfo.RentInPay;
        //                                if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                                    UseMonthlyRent = carInfo.mFinal;
        //                                Discount = carInfo.useDisc;
        //                            }
        //                        }

        //                        if (UseMonthlyRent.Count > 0)
        //                        {
        //                            UseMonthMode = true;
        //                            int UseLen = UseMonthlyRent.Count;
        //                            for (int i = 0; i < UseLen; i++)
        //                            {
        //                                //flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours), Convert.ToInt32(UseMonthlyRent[i].HolidayHours), 0, LogID, ref errCode); //寫入記錄
        //                                flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, LogID, ref errCode); //寫入記錄
        //                            }
        //                        }
        //                        else
        //                        {
        //                            UseMonthMode = false;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //        #region 開始計價
        //        if (flag)
        //        {
        //            lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
        //            if (ProjType == 4)
        //            {
        //                if (UseMonthMode)   //true:有月租;false:無月租
        //                {
        //                    outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
        //                    outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
        //                }
        //                else
        //                {
        //                    var item = OrderDataLists[0];
        //                    var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

        //                    carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, null, null, Discount);
        //                    if (carInfo != null)
        //                        outputApi.Rent.CarRental = carInfo.RentInPay;
        //                }

        //                outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
        //            }
        //            else
        //            {
        //                int BaseMinutes = 60;
        //                int tmpTotalRentMinutes = TotalRentMinutes;

        //                if (TotalRentMinutes < BaseMinutes)
        //                {
        //                    TotalRentMinutes = BaseMinutes;
        //                }
        //                if (UseMonthMode)
        //                {
        //                    TotalRentMinutes -= Convert.ToInt32((billCommon._scriptHolidayHour + billCommon._scriptWorkHour) * 60);
        //                    if (TotalRentMinutes < 0)
        //                    {
        //                        TotalRentMinutes = 0;
        //                    }
        //                }
        //                if (TotalPoint >= TotalRentMinutes)
        //                {
        //                    ActualRedeemableTimePoint = TotalRentMinutes;
        //                }
        //                else
        //                {
        //                    if ((TotalPoint - TotalRentMinutes) < 30)
        //                    {
        //                        ActualRedeemableTimePoint = TotalRentMinutes - 30;
        //                    }
        //                }

        //                #region 非月租折扣計算
        //                //note: 折扣計算
        //                //double wDisc = 0;
        //                //double hDisc = 0;
        //                //int PayDisc = 0;
        //                if (!UseMonthMode)
        //                {
        //                    if (hasFine)
        //                    {
        //                        var xre = new BillCommon().CarDiscToPara(SD, ED, carBaseMins, 600, lstHoliday, Discount);
        //                        if (xre != null)
        //                        {
        //                            nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
        //                            nor_car_wDisc = xre.Item2;
        //                            nor_car_hDisc = xre.Item3;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var xre = new BillCommon().CarDiscToPara(SD, FED, carBaseMins, 600, lstHoliday, Discount);
        //                        if (xre != null)
        //                        {
        //                            nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
        //                            nor_car_wDisc = xre.Item2;
        //                            nor_car_hDisc = xre.Item3;
        //                        }
        //                    }

        //                    var discPrice = Convert.ToDouble(car_n_price) * (nor_car_wDisc / 60) + Convert.ToDouble(car_h_price) * (nor_car_hDisc / 60);
        //                    nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
        //                    Discount = nor_car_PayDisc;
        //                }

        //                #endregion

        //                if (TotalRentMinutes > 0)
        //                {
        //                    TotalRentMinutes -= Discount;
        //                }
        //                else
        //                {
        //                    TotalRentMinutes = 0;
        //                }

        //                if (UseMonthMode)
        //                {
        //                    outputApi.Rent.CarRental = CarRentPrice;
        //                    outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
        //                    outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
        //                }
        //                else
        //                {
        //                    CarRentPrice = car_inPrice;//未逾時租用費用
        //                    if (hasFine)
        //                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
        //                }

        //                if (Discount > 0)
        //                {
        //                    //var result = new BillCommon().GetCarRangeMins(SD, ED, 60, 10 * 60, lstHoliday);
        //                    //int DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE)));

        //                    double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
        //                    double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

        //                    if (UseMonthMode)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        //非月租折扣
        //                        CarRentPrice -= nor_car_PayDiscPrice;
        //                        CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
        //                    }
        //                }
        //                //安心服務
        //                InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
        //                if (InsurancePerHours > 0)
        //                {
        //                    outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

        //                    //逾時安心服務計算
        //                    if (TotalFineRentMinutes > 0)
        //                    {
        //                        outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
        //                    }
        //                }

        //                outputApi.Rent.CarRental = CarRentPrice;
        //                outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
        //                outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
        //                //里程費計算修改，遇到取不到里程數的先以0元為主
        //                //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
        //                // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
        //                if (OrderDataLists[0].start_mile == 0 ||
        //                    OrderDataLists[0].end_mile == 0 ||
        //                    ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) > 1000) ||
        //                    ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0)
        //                    )
        //                {
        //                    outputApi.Rent.MileageRent = 0;
        //                }
        //                else
        //                {
        //                    outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
        //                }
        //            }

        //            outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
        //            outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
        //            outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
        //            //20201202 ADD BY ADAM REASON.ETAG費用
        //            outputApi.Rent.ETAGRental = etagPrice;

        //            var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
        //            xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
        //            outputApi.Rent.TotalRental = xTotalRental;

        //            #region 修正輸出欄位

        //            var tra = OrderDataLists[0].init_TransDiscount;
        //            if (xTotalRental == 0)
        //            {
        //                var carPri = outputApi.Rent.CarRental;
        //                if (carPri > 0)
        //                    outputApi.Rent.TransferPrice = carPri;
        //            }

        //            //note: 修正輸出欄位PayDetail
        //            if (ProjType == 4)
        //            {
        //                outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
        //                outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
        //                outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

        //                //2020-12-29 所有點數改成皆可折抵
        //                //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
        //                outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

        //                outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

        //                var cDisc = apiInput.Discount;
        //                var mDisc = apiInput.MotorDiscount;
        //                if (carInfo.useDisc > 0)
        //                {
        //                    int lastDisc = carInfo.useDisc;
        //                    var useMdisc = mDisc > carInfo.useDisc ? carInfo.useDisc : mDisc;
        //                    lastDisc -= useMdisc;
        //                    gift_motor_point = useMdisc;
        //                    if (lastDisc > 0)
        //                    {
        //                        var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
        //                        lastDisc -= useCdisc;
        //                        gift_point = useCdisc;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (UseMonthMode)
        //                {
        //                    outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
        //                    outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
        //                    outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

        //                    //2020 - 12 - 29 所有點數改成皆可折抵
        //                    //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
        //                    outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

        //                    outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
        //                    if (carInfo != null && carInfo.useDisc > 0)
        //                        gift_point = carInfo.useDisc;
        //                }
        //                else
        //                {
        //                    outputApi.Rent.UseNorTimeInterval = Discount.ToString();
        //                    outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
        //                    outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_pay_in_wMins + car_pay_in_hMins).ToString();//可折抵租用時數
        //                    outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
        //                    gift_point = nor_car_PayDisc;
        //                }

        //                gift_motor_point = 0;
        //                outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
        //            }

        //            #endregion

        //            string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice);
        //            SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
        //            {
        //                IDNO = IDNO,
        //                OrderNo = tmpOrder,
        //                final_price = outputApi.Rent.TotalRental,
        //                pure_price = outputApi.Rent.CarRental,
        //                mileage_price = outputApi.Rent.MileageRent,
        //                Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
        //                fine_price = outputApi.Rent.OvertimeRental,
        //                //gift_point = apiInput.Discount,
        //                //gift_motor_point = apiInput.MotorDiscount,
        //                gift_point = gift_point,
        //                gift_motor_point = gift_motor_point,

        //                Etag = outputApi.Rent.ETAGRental,
        //                parkingFee = outputApi.Rent.ParkingFee,
        //                TransDiscount = outputApi.Rent.TransferPrice,
        //                Token = Access_Token,
        //                LogID = LogID,
        //            };

        //            SPOutput_Base SPOutput = new SPOutput_Base();
        //            SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
        //            flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
        //            baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
        //        }
        //        #endregion
        //    }
        //    #endregion

        //    #region 寫入錯誤Log
        //    if (false == flag && false == isWriteError)
        //    {
        //        baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
        //    }
        //    #endregion
        //    #region 輸出
        //    baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
        //    return objOutput;
        //    #endregion
        //}
        #endregion

    }

}