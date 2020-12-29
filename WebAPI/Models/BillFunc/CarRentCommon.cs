using Domain.SP.Input.Rent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Enum;
using Domain.SP.Output;
using WebCommon;
using Domain.SP.Output.OrderList;
using System.Data;
using WebAPI.Models.BaseFunc;
using Reposotory.Implement;
using System.Configuration;
using Domain.TB;

namespace WebAPI.Models.BillFunc
{
    public class CarRentCommon
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        //public OBIZ_GetPayDetail GetPayDetail(IBIZ_GetPayDetail sour)
        //{
        //    var re = new OBIZ_GetPayDetail();

        //    var baseVerify = new CommonFunc();
        //    var lstError = new List<ErrorInfo>();
        //    bool flag = true;
        //    string errCode = "000000"; //預設成功
        //    int ProjType = 0;
        //    DateTime SD = new DateTime();
        //    DateTime ED = new DateTime();
        //    DateTime FED = new DateTime();
        //    List<Holiday> lstHoliday = null; //假日列表
        //    bool hasFine = false; //是否逾時

        //    #region 取出訂單資訊

        //    SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
        //    {
        //        IDNO = sour.IDNO,
        //        OrderNo = sour.OrderNo,
        //        LogID =  sour.LogID,
        //        Token = sour.Token
        //    };
        //    string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
        //    SPOutput_Base spOutBase = new SPOutput_Base();
        //    SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
        //    var OrderDataLists = new List<OrderQueryFullData>();
        //    DataSet ds = new DataSet();
        //    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
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

        //    #endregion

        //    #region 第二階段判斷及計價
        //    if (flag)
        //    {
        //        //判斷狀態
        //        if (OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
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
        //                    var reInMins = billCommon.GetCarRangeMins(SD, ED, 60, 600, lstHoliday);
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
        //                    var reAllMins = billCommon.GetCarRangeMins(SD, FED, 60, 600, lstHoliday);
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
        //                        carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, item.BaseMinutes, dayMaxMinns, lstHoliday, motoMonth, motoDisc);

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
        //                            carInfo = billCommon.CarRentInCompute(SD, ED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, OrderDataLists[0].BaseMinutes, 10, lstHoliday, UseMonthlyRent, xDiscount);
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
        //                            carInfo = billCommon.CarRentInCompute(SD, FED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, OrderDataLists[0].BaseMinutes, 10, lstHoliday, UseMonthlyRent, xDiscount);
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

        //                    carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, item.BaseMinutes, dayMaxMinns, null, null, Discount);
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
        //                        var xre = new BillCommon().CarDiscToPara(SD, ED, 60, 600, lstHoliday, Discount);
        //                        if (xre != null)
        //                        {
        //                            nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
        //                            nor_car_wDisc = xre.Item2;
        //                            nor_car_hDisc = xre.Item3;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var xre = new BillCommon().CarDiscToPara(SD, FED, 60, 600, lstHoliday, Discount);
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
        //                outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
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
        //                    outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
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

        //    return re;
        //}

    }

    public class IBIZ_GetPayDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { set; get; }
        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }

        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }

        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }

        public Int64 LogID { set; get; }
    }

    public class OBIZ_GetPayDetail : OAPI_GetPayDetail
    {
        public List<ErrorInfo> lstError { get; set; }
        public bool flag { get; set; }
        public string errCode { get; set; } = "000000"; //預設成功

    }
}