﻿using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
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
using WebCommon;

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
            DateTime FineDate = new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;  //false:無月租;true:有月租
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetPayDetail>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
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
                if (flag)
                {
                    if (apiInput.Discount < 0)
                    {
                        flag = false;
                        errCode = "ERR202";
                    }

                    if (apiInput.MotorDiscount < 0)
                    {
                        flag = false;
                        errCode = "ERR202";
                    }

                    Discount = apiInput.Discount + apiInput.MotorDiscount;
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
            }
            #endregion
            #region 取出基本資料
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
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
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
                //判斷狀態
                if (OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
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
                    ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                    ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                    FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (FED.Subtract(ED).Ticks > 0)
                    {
                        FineDate = ED;
                        hasFine = true;
                        billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                        billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
                        TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數
                    }
                    else
                    {
                        billCommon.CalDayHourMin(SD, FED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                    }
                }
                if (flag)
                {
                    if (NowTime.Subtract(FED).TotalMinutes >= 30)
                    {
                        flag = false;
                        errCode = "ERR208";
                    }
                }
                #region 與短租查時數
                if (flag)
                {
                    WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                    HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                    flag = wsAPI.NPR270Query(IDNO, ref wsOutput);
                    if (flag)
                    {
                        int giftLen = wsOutput.Data.Length;

                        if (giftLen > 0)
                        {
                            for (int i = 0; i < giftLen; i++)
                            {
                                DateTime tmpDate;
                                int tmpPoint = 0;
                                bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                                bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                                if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                                {
                                    if (wsOutput.Data[i].GIFTTYPE == "01")
                                    {
                                        MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                    }
                                    else
                                    {
                                        CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                        errCode = "0000";
                    }
                    //判斷輸入的點數有沒有超過總點數
                    if (ProjType == 4)
                    {
                        if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)
                        {
                            flag = false;
                            errCode = "ERR205";
                        }
                        else
                        {
                            if (Discount > (MotorPoint + CarPoint))
                            {
                                flag = false;
                                errCode = "ERR207";
                            }
                        }

                        if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > 使用時數
                        {
                            flag = false;
                            errCode = "ERR303";
                        }

                        if (flag)
                        {
                            billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
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
                        if (flag)
                        {
                            billCommon.CalPointerToDayHourMin(CarPoint, ref PDays, ref PHours, ref PMins);
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
                }
                #endregion
                #region 月租
                if (flag)
                {
                    //1.0 先還原這個單號使用的
                    flag = monthlyRentRepository.RestoreHistory(IDNO, tmpOrder, LogID, ref errCode);
                    int RateType = (ProjType == 4) ? 1 : 0;
                    if (hasFine)
                    {
                        monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                    }
                    else
                    {
                        monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                    }
                    int MonthlyLen = monthlyRentDatas.Count;
                    if (MonthlyLen > 0)
                    {
                        UseMonthMode = true;

                        if (flag)
                        {
                            if (ProjType == 4)
                            {
                                //機車沒有分平假日，直接送即可
                                for (int i = 0; i < MonthlyLen; i++)
                                {
                                    //int MotoTotalHours = Convert.ToInt32(60 * monthlyRentDatas[i].MotoTotalHours); //20201102月租改為以分鐘數回傳
                                    int MotoTotalHours = Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);
                                    if (MotoTotalHours >= TotalRentMinutes) //全部扣光
                                    {
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, TotalRentMinutes, LogID, ref errCode);//寫入記錄
                                        TotalRentMinutes = 0;
                                        break;
                                    }
                                    else
                                    {
                                        if (TotalRentMinutes - MotoTotalHours >= OrderDataLists[0].BaseMinutes) //扣完有超過基本費
                                        {
                                            TotalRentMinutes -= MotoTotalHours;
                                            flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, MotoTotalHours, LogID, ref errCode); //寫入記錄
                                        }
                                        else
                                        {
                                            MotoTotalHours += TotalRentMinutes - MotoTotalHours - OrderDataLists[0].BaseMinutes;
                                            TotalRentMinutes -= MotoTotalHours;
                                            flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, MotoTotalHours, LogID, ref errCode); //寫入記錄
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();
                                if (hasFine)
                                {
                                    CarRentPrice = billCommon.CalBillBySubScription(SD, ED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                                }
                                else
                                {
                                    CarRentPrice = billCommon.CalBillBySubScription(SD, FED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                                }
                                if (UseMonthlyRent.Count > 0)
                                {
                                    UseMonthMode = true;
                                    int UseLen = UseMonthlyRent.Count;
                                    for (int i = 0; i < UseLen; i++)
                                    {
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, LogID, ref errCode); //寫入記錄
                                    }
                                }
                                else
                                {
                                    UseMonthMode = false;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 開始計價
                if (flag)
                {
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (ProjType == 4)
                    {
                        if (TotalPoint >= TotalRentMinutes) //可使用總點數 >= 總租車時數
                        {
                            ActualRedeemableTimePoint = TotalRentMinutes;
                        }
                        else
                        {
                            if ((TotalPoint - TotalRentMinutes) < OrderDataLists[0].BaseMinutes)
                            {
                                ActualRedeemableTimePoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
                            }
                        }
                        if (Discount >= TotalRentMinutes)   // 要折抵的點數 >= 總租車時數
                        {
                            Discount = (days * 600) + (hours * 60) + (mins);    //自動縮減
                        }
                        else
                        {
                            int tmp = TotalRentMinutes - Discount;
                            if (tmp < OrderDataLists[0].BaseMinutes)
                            {
                                Discount += TotalRentMinutes - Discount - OrderDataLists[0].BaseMinutes;
                            }
                        }
                        TotalRentMinutes -= Discount;   // 總租車時數 = 總租車時數 - 要折抵的點數

                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, monthlyRentDatas[0].WorkDayRateForMoto, monthlyRentDatas[0].HoildayRateForMoto, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                        }
                        else
                        {
                            billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                        }

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    }
                    else
                    {
                        int BaseMinutes = 60;
                        int tmpTotalRentMinutes = TotalRentMinutes;

                        if (TotalRentMinutes < BaseMinutes)
                        {
                            TotalRentMinutes = BaseMinutes;
                        }
                        if (UseMonthMode)
                        {
                            TotalRentMinutes -= Convert.ToInt32((billCommon._scriptHolidayHour + billCommon._scriptWorkHour) * 60);
                            if (TotalRentMinutes < 0)
                            {
                                TotalRentMinutes = 0;
                            }
                        }
                        if (TotalPoint >= TotalRentMinutes)
                        {
                            ActualRedeemableTimePoint = TotalRentMinutes;
                        }
                        else
                        {
                            if ((TotalPoint - TotalRentMinutes) < 30)
                            {
                                ActualRedeemableTimePoint = TotalRentMinutes - 30;
                            }
                        }
                        if (Discount > TotalRentMinutes)
                        {
                            Discount = (days * 600) + (hours * 60);        //自動縮減
                            if (mins > 15 && mins < 45)
                            {
                                Discount += 30;
                            }
                            else if (mins > 45)
                            {
                                Discount += 60;
                            }
                        }
                        else
                        {
                            int tmp = TotalRentMinutes - Discount;
                            if (tmp > 0 && tmp < 30)
                            {
                                Discount += TotalRentMinutes - Discount - 30;
                            }
                        }

                        if (TotalRentMinutes > 0)
                        {
                            TotalRentMinutes -= Discount;
                        }
                        else
                        {
                            TotalRentMinutes = 0;
                        }

                        if (UseMonthMode)
                        {
                            outputApi.Rent.CarRental = CarRentPrice;
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
                        }
                        else
                        {
                            if (hasFine)
                            {
                                CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, ED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));
                            }
                            else
                            {
                                CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, FED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));
                            }
                        }
                        if (Discount > 0)
                        {
                            int DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE)));
                            if (UseMonthMode)
                            {
                                if (billCommon._scriptHolidayHour > 0)
                                {
                                    DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * monthlyRentDatas[0].HoildayRateForCar)));
                                }
                                else
                                {
                                    DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * monthlyRentDatas[0].WorkDayRateForCar)));
                                }
                            }
                            else
                            {
                                if (billCommon._holidayHour > 0)
                                {
                                    DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE_H)));
                                }
                            }
                            CarRentPrice -= DiscountPrice;
                        }
                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    outputApi.Rent.TotalRental = (outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice < 0) ? 0 : outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice;
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
                        gift_point = apiInput.Discount,
                        gift_motor_point = apiInput.MotorDiscount,
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        Token = Access_Token,
                        LogID = LogID,
                    };
                    SPOutput_Base SPOutput = new SPOutput_Base();
                    SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                }
                #endregion
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
    }
}