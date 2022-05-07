using Domain.Flow.CarRentCompute;
using Domain.SP.Input.Bill;
using Domain.SP.Output.Bill;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebAPI.Models.BaseFunc;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 計費公用程式
    /// </summary>
    public class BillCommon
    {
        public delegate double MinsProcess(double mins);//剩餘分轉計費分(未滿60)
        public delegate void DayMinsProcess(ref double wMins, ref double hMins);//當日分特殊邏輯

        #region 計算時間
        /// <summary>
        /// 計算時間
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="Days">天數</param>
        /// <param name="Hours">時數</param>
        /// <param name="Minutes">分數</param>
        public void CalDayHourMin(DateTime SD, DateTime ED, ref int Days, ref int Hours, ref int Minutes)
        {
            double totalHours = Math.Floor(ED.Subtract(SD).TotalHours);
            Minutes = ED.Subtract(SD).Minutes;
            Days = Convert.ToInt32(Math.Floor(totalHours / 24));
            Hours = Convert.ToInt32(totalHours % 24);
            //2017-01-24新增，避免尾差
            if (Hours >= 10)
            {
                Days += 1;
                Hours = 0;
                Minutes = 0;
            }
        }
        #endregion

        #region 將折抵時數換算成天、時、分
        /// <summary>
        /// 將折抵時數換算成天、時、分
        /// </summary>
        /// <param name="point">總點數</param>
        /// <param name="Days"></param>
        /// <param name="Hours"></param>
        /// <param name="Minutes"></param>
        public void CalPointerToDayHourMin(int point, ref int Days, ref int Hours, ref int Minutes)
        {
            Days = Convert.ToInt32(Math.Floor(Convert.ToDouble(point / 600)));
            Hours = Convert.ToInt32(Math.Floor(Convert.ToDouble(((point % 600) / 60))));
            Minutes = point - (Days * 600) - (Hours * 60);
        }
        #endregion

        #region 取出每公里n元
        /// <summary>
        /// 取出每公里n元
        /// </summary>
        /// <param name="ProjID">專案代碼</param>
        /// <param name="CarType">車型</param>
        /// <param name="SDate">起日</param>
        /// <param name="EDate">迄日</param>
        /// <param name="LogID">此筆呼叫的id</param>
        /// <returns></returns>
        public float GetMilageBase(string ProjID, string CarType, DateTime SDate, DateTime EDate, Int64 LogID)
        {
            bool flag = true;
            float MilageBase = -1;
            string errCode = "";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPInput_GetMilageSetting SPInput = new SPInput_GetMilageSetting()
            {
                ProjID = ProjID,
                CarType = CarType,
                EDate = EDate,
                SDate = SDate,
                LogID = LogID
            };
            SPOutput_GetMilageSetting SPOutput = new SPOutput_GetMilageSetting();

            try
            {
                string SPName = "usp_GetMilageSetting";
                SQLHelper<SPInput_GetMilageSetting, SPOutput_GetMilageSetting> sqlHelp = new SQLHelper<SPInput_GetMilageSetting, SPOutput_GetMilageSetting>(WebApiApplication.connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                new CommonFunc().checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    MilageBase = SPOutput.MilageBase;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return MilageBase;
        }
        #endregion

        #region 計算租金
        /// <summary>
        /// 計算租金
        /// </summary>
        /// <param name="mins">分</param>
        /// <param name="hours">時</param>
        /// <param name="Price">平日價</param>
        /// <param name="PriceH">假日價</param>
        /// <param name="DateStr">日期</param>
        /// <param name="lstHoliday">假日清單</param>
        /// <returns></returns>
        public int calPay(int mins, double hours, int Price, int PriceH, string DateStr, List<Holiday> lstHoliday)
        {
            if (mins <= 0 && hours == 0)
            {
                return 0;
            }
            else if (mins <= 15)
            {
                mins = 0;
            }
            else if (mins > 15 && mins <= 45)
            {
                hours += 0.5;
                mins = 0;
            }
            else if (mins > 45)
            {
                hours += 1;
                mins = 0;
            }
            double tmpPay = 0;
            double totalPay = 0;
            int HolidayIndex = lstHoliday.FindIndex(delegate (Holiday hd)
            {
                return hd.HolidayDate == DateStr;
            });
            if (HolidayIndex >= 0)
            {
                tmpPay = hours * (PriceH / 10);
                totalPay = (tmpPay > PriceH) ? PriceH : tmpPay;
            }
            else
            {
                tmpPay = hours * (Price / 10);
                totalPay = (tmpPay > Price) ? Price : tmpPay;
            }
            return Convert.ToInt32(totalPay);
        }
        #endregion

        #region 租金試算
        /// <summary>
        /// 租金試算
        /// </summary>
        /// <param name="SD">預計取車時間</param>
        /// <param name="ED">預計還車時間</param>
        /// <param name="Price">平日費用</param>
        /// <param name="PriceH">假日費用</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <returns></returns>
        public double CalSpread(DateTime SD, DateTime ED, int Price, int PriceH, List<Holiday> lstHoliday)
        {
            double totalPay = 0;

            double totalHours = Math.Floor(ED.Subtract(SD).TotalHours);
            int totalMinutes = ED.Subtract(SD).Minutes;
            double Day = Math.Floor(totalHours / 24);
            double tHours = totalHours % 24;

            if (tHours >= 10)
            {
                Day += 1;
                tHours = 0;
                totalMinutes = 0;
            }
            if (Day >= 1)
            {
                for (int i = 0; i < Day; i++)
                {
                    totalPay += calPay(0, 24, Price, PriceH, SD.AddHours((i * 24d)).ToString("yyyyMMdd"), lstHoliday);
                }
                if (tHours > 0)
                {
                    totalPay += calPay(0, tHours, Price, PriceH, SD.AddHours((Day * 24d)).ToString("yyyyMMdd"), lstHoliday);
                }
                if (totalMinutes > 0)
                {
                    totalPay += calPay(totalMinutes, 0, Price, PriceH, SD.AddHours((Day * 24d)).ToString("yyyyMMdd"), lstHoliday);
                }
            }
            else if (Day == 0)
            {
                if (tHours < 10)
                {
                    double diffTotalHours = Math.Floor(ED.Subtract(SD).TotalHours); //總時數
                    int diffTotalMinus = ED.Subtract(SD).Minutes; //總分鐘數
                    if (diffTotalHours > 1)
                    {
                        if (SD.Date == ED.Date)
                        {
                            totalPay += calPay(diffTotalMinus, diffTotalHours, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                        }
                        else
                        {
                            DateTime ttSD = SD.AddDays(1);
                            DateTime tmpSD = new DateTime(ttSD.Year, ttSD.Month, ttSD.Day, 0, 0, 0);
                            DateTime tmpED = new DateTime(ED.Year, ED.Month, ED.Day, 0, 0, 0);
                            double totalSDHours = Math.Floor(tmpSD.Subtract(SD).TotalHours);
                            int totalSDMinute = tmpSD.Subtract(SD).Minutes;
                            double totalEDHours = Math.Floor(ED.Subtract(tmpED).TotalHours);
                            int totalEDMinute = ED.Subtract(tmpED).Minutes;

                            int tmp = 60 - totalSDMinute;
                            if (totalEDMinute >= tmp)
                            {
                                totalEDMinute -= tmp;
                                totalPay += calPay(0, totalSDHours + 1, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                                totalPay += calPay(totalEDMinute, totalEDHours, Price, PriceH, ED.ToString("yyyyMMdd"), lstHoliday);
                            }
                            else
                            {
                                totalEDMinute = totalEDMinute + 60 - tmp;
                                totalEDHours -= 1;
                                totalPay += calPay(0, totalSDHours + 1, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                                totalPay += calPay(totalEDMinute, totalEDHours, Price, PriceH, ED.ToString("yyyyMMdd"), lstHoliday);
                            }
                        }
                    }
                    else if (diffTotalHours == 1 && diffTotalMinus == 0)
                    {
                        totalPay += calPay(0, 1, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                    }
                    else if (diffTotalHours == 1 && diffTotalMinus > 0)
                    {
                        totalPay += calPay(0, 1, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                        totalPay += calPay(diffTotalMinus, 0, Price, PriceH, ED.ToString("yyyyMMdd"), lstHoliday);
                    }
                    else if (diffTotalHours < 1)
                    {
                        totalPay += calPay(0, 1, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                    }
                }
                else
                {
                    //大於十小時，小於1天
                    totalPay += calPay(totalMinutes, tHours, Price, PriceH, SD.ToString("yyyyMMdd"), lstHoliday);
                }
            }
            return totalPay;
        }
        #endregion

        #region 區間租金計算,可包含多月租,一般平假日,前n免費
        /// <summary>
        /// 區間租金計算,可包含多月租,一般平假日,前n免費
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="priceN">專案平日99</param>
        /// <param name="priceH">專案假日168</param>
        /// <param name="daybaseMins">基本分鐘60/param>
        /// <param name="dayMaxHour">計費單日最大小時數10</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <param name="mOri">月租列表</param>
        /// <param name="Discount">折扣</param>
        /// <param name="FreeMins">前n免費</param>
        /// <param name="GiveMinute">標籤優惠分鐘數</param>
        public CarRentInfo CarRentInCompute(DateTime SD, DateTime ED, double priceN, double priceH, double daybaseMins, double dayMaxHour,
            List<Holiday> lstHoliday, List<MonthlyRentData> mOri, int Discount, double FreeMins = 0, int GiveMinute = 0
            )
        {
            int DayBaseMinute = Convert.ToInt32(daybaseMins);

            if (SD == null || ED == null || SD > ED)
                throw new Exception("SD,ED錯誤");

            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);
            var mins = ED.Subtract(SD).TotalMinutes;

            if (mins >= daybaseMins)
                daybaseMins = 0;

            CarRentInfo re = new CarRentInfo();
            double dre = 0;
            double lastDisc = Convert.ToDouble(Discount);//剩餘折扣
            List<MonthlyRentData> mFinal = new List<MonthlyRentData>();//剩餘月租點數
            List<string> norDates = new List<string>()//一般平假日
            {
                eumDateType.wDay.ToString(),
                eumDateType.hDay.ToString()
            };

            if (mOri != null && mOri.Count() > 0)
            {
                if (mOri.Any(x => x.WorkDayHours < 0 || x.WorkDayRateForCar < 0 || x.HolidayHours < 0 || x.HoildayRateForCar < 0 || x.MonthlyRentId <= 0 || x.Mode != 0))
                    throw new Exception("mOri資料內容錯誤");

                if (mOri.GroupBy(x => x.MonthlyRentId).Where(y => y.Count() > 1).Count() > 0)
                    throw new Exception("MonthlyRentId不可重複");

                mOri = mOri.OrderByDescending(x => x.MonLvl).ToList();

                mFinal = objUti.Clone(mOri);

                //小時轉分
                mFinal.ForEach(x =>
                {
                    x.CarTotalHours = x.CarTotalHours * 60;
                    x.WorkDayHours = x.WorkDayHours * 60;
                    x.HolidayHours = x.HolidayHours * 60;
                });
            }

            var allDay = GetDateMark(SD, ED, lstHoliday, mOri); //區間內時間註記
            var dayPayList = GetCarTypeMins(SD, ED, daybaseMins, dayMaxHour * 60, allDay);  //全分類時間

            if (FreeMins > 0 && dayPayList != null && dayPayList.Count() > 0)
                dayPayList = befMinsFree(FreeMins, dayPayList);

            re.RentInMins = Convert.ToInt32(dayPayList.Select(x => x.xMins).Sum());     // 扣掉免費時數後記錄總租用時數

            var norList = dayPayList.Where(x => norDates.Any(y => y == x.DateType)).ToList();   //一般時段
            var dpList = new List<DayPayMins>();    //剩餘有分鐘數的

            #region 費率回存
            if (dayPayList != null && dayPayList.Count() > 0)
            {
                dayPayList.ForEach(x =>
                {
                    if (x.DateType == eumDateType.wDay.ToString())
                        x.xRate = priceN;
                    else if (x.DateType == eumDateType.hDay.ToString())
                        x.xRate = priceH;
                    else if (x.DateType.Contains("h"))
                    {
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType.Replace("h", string.Empty).Trim()).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.HoildayRateForCar;
                    }
                    else
                    {
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.WorkDayRateForCar;
                    }
                });
            }
            #endregion

            double wDisc = 0; //平日折扣
            double hDisc = 0; //假日折扣 
            double m_wDisc = 0; //平日折扣(所有月租)
            double m_hDisc = 0; //假日折扣(所有月租)

            double UseGiveMinute = 0;
            if (GiveMinute > 0)
            {
                var BillingTime = Convert.ToInt32(dayPayList.Select(x => x.xMins).Sum());   // 收費時數
                if (BillingTime > DayBaseMinute)   // 收費時數 > 基本消費分鐘數 才可使用標籤優惠時數
                {
                    double RemainGiveMinute = Convert.ToDouble(GiveMinute);

                    //價高先折
                    dpList = dayPayList.Where(x => x.xMins > 0).OrderByDescending(x => x.xRate).ThenBy(x => x.xDate).ToList();

                    dpList.ForEach(x =>
                    {
                        if (RemainGiveMinute > 0)
                        {
                            var useDisc = RemainGiveMinute > x.xMins ? x.xMins : RemainGiveMinute;
                            RemainGiveMinute -= useDisc;
                            x.xMins -= useDisc;
                            UseGiveMinute += useDisc;
                        }
                    });
                }
            }

            var mList = dayPayList.Where(x => !norDates.Any(y => y == x.DateType)).OrderBy(z => z.xDate).ToList();
            var mDiscs = new List<DayPayMins>();//有變動的日期 

            #region 訂閱制點數先折
            if (mFinal != null && mFinal.Count() > 0)
            {
                foreach (var m in mFinal)
                {
                    var carAllDisc = Convert.ToDouble(m.CarTotalHours);
                    var wdisc = Convert.ToDouble(m.WorkDayHours - m.WorkDayHours % 30); //平日可折
                    var hdisc = Convert.ToDouble(m.HolidayHours - m.HolidayHours % 30); //假日可折

                    var allDays = mList.Where(x => x.DateType == m.MonthlyRentId.ToString() || x.DateType == (m.MonthlyRentId.ToString() + "h")).OrderBy(y => y.xDate).ToList();
                    var wDays = mList.Where(x => x.DateType == m.MonthlyRentId.ToString()).OrderBy(y => y.xDate).ToList();
                    var hDays = mList.Where(x => x.DateType == (m.MonthlyRentId.ToString() + "h")).OrderBy(y => y.xDate).ToList();

                    if (carAllDisc > 0)
                    {
                        allDays.ForEach(x =>
                        {
                            if (carAllDisc > 0)
                            {
                                var useDisc = carAllDisc > x.xMins ? x.xMins : carAllDisc;
                                carAllDisc -= useDisc;
                                x.xMins = x.xMins - useDisc;
                                mDiscs.Add(x);

                                if (x.DateType.Contains("h"))
                                    m_hDisc += useDisc;
                                else
                                    m_wDisc += useDisc;
                            }
                        });

                        m.CarTotalHours -= (Convert.ToSingle(carAllDisc));//使用點數(原始點數-剩餘點數)
                    }
                    else
                    {
                        wDays.ForEach(x =>
                        {
                            if (wdisc > 0)
                            {
                                var useDisc = wdisc > x.xMins ? x.xMins : wdisc;
                                wdisc -= useDisc;
                                x.xMins = x.xMins - useDisc;
                                mDiscs.Add(x);
                                m_wDisc += useDisc;
                            }
                        });

                        hDays.ForEach(x =>
                        {
                            if (hdisc > 0)
                            {
                                var useDisc = hdisc > x.xMins ? x.xMins : hdisc;
                                hdisc -= useDisc;
                                x.xMins = x.xMins - useDisc;
                                mDiscs.Add(x);
                                m_hDisc += useDisc;
                            }
                        });

                        m.WorkDayHours -= Convert.ToSingle(wdisc);//使用點數(原始點數-剩餘點數)
                        m.HolidayHours -= Convert.ToSingle(hdisc);//使用點數(原始點數-剩餘點數)
                    }
                }

                //異動回存
                if (mDiscs != null && mDiscs.Count() > 0)
                {
                    dayPayList.ForEach(x =>
                    {
                        var md = mDiscs.Where(y => y.xDate == x.xDate && y.DateType == x.DateType).FirstOrDefault();
                        if (md != null)
                            x = md;
                    });
                }
            }
            #endregion

            #region 混和折扣前先記錄使用月租點數
            re.useMonthDisc = m_wDisc + m_hDisc;
            re.useMonthDiscW = m_wDisc;
            re.useMonthDiscH = m_hDisc;
            #endregion

            //價高先折
            dpList = dayPayList.Where(z => z.xMins > 0).OrderByDescending(x => x.xRate).ThenBy(y => y.xDate).ToList();
            dpList.ForEach(x =>
            {
                if (lastDisc > 0)
                {
                    var useDisc = lastDisc > x.xMins ? x.xMins : lastDisc;
                    lastDisc -= useDisc;
                    x.xMins = x.xMins - useDisc;
                    if (x.DateType == eumDateType.wDay.ToString())
                        wDisc += useDisc;
                    else if (x.DateType == eumDateType.hDay.ToString())
                        hDisc += useDisc;
                    else if (x.DateType.Contains("h"))
                        m_hDisc += useDisc;
                    else
                        m_wDisc += useDisc;
                }
            });

            dpList = dpList.Where(x => x.xMins > 0).ToList();

            if (dpList != null && dpList.Count() > 0)
            {
                //分類統計
                var xre = dpList.GroupBy(x => x.DateType).Select(y => new DayPayMins { DateType = y.Key, xMins = y.Select(z => z.xMins).Sum(), xRate = y.Select(w => w.xRate).FirstOrDefault() }).ToList();

                //折扣後租用時數
                re.AfterDiscRentInMins = Convert.ToInt32(xre.Select(x => x.xMins).Sum());

                //租金計算
                xre.ForEach(x => dre += (x.xMins / 60) * x.xRate);
            }

            re.useDisc = Convert.ToInt32(Convert.ToDouble(Discount) - lastDisc);

            //原始總點數-使用總點數
            re.lastMonthDisc = mOri.Select(x => x.WorkDayHours * 60 + x.HolidayHours * 60 + x.CarTotalHours * 60).Sum() 
                                                - mFinal.Select(x => x.WorkDayHours * 60 + x.HolidayHours * 60 + x.CarTotalHours * 60).Sum();

            if (mFinal != null && mFinal.Count() > 0)//回傳monthData
            {
                mFinal.ForEach(x =>
                {
                    x.CarTotalHours = (x.CarTotalHours / 60);//使用的小時數,分轉回小時
                    x.HolidayHours = (x.HolidayHours / 60);//使用的小時數,分轉回小時
                    x.WorkDayHours = (x.WorkDayHours / 60);//使用的小時數,分轉回小時 
                });
                re.mFinal = mFinal;
            }

            re.UseGiveMinute = Convert.ToInt32(UseGiveMinute);
            // 20220414 UPD BY YEH REASON:此欄位改為記錄 實際租用時數 - 訂閱制時數 - 標籤優惠時數，用意是記錄一個未折抵的數字，讓APP在部分折抵可以使用
            re.DiscRentInMins = re.RentInMins - Convert.ToInt32(re.useMonthDisc + UseGiveMinute);

            dre = dre > 0 ? dre : 0;
            re.RentInPay = Convert.ToInt32(Math.Round(dre, 0, MidpointRounding.AwayFromZero));
            return re;
        }
        #endregion

        #region 機車月租計算,區分平假日,不分平假日
        /// <summary>
        /// 機車月租計算,區分平假日,不分平假日
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="priceNmin">平日價格</param>
        /// <param name="priceHmin">假日價格</param>
        /// <param name="dayBaseMins">一日基本分鐘</param>
        /// <param name="dayMaxMins">一日最大分鐘數</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <param name="mOri">月租</param>
        /// <param name="Discount">折扣</param>
        /// <param name="fDayMaxMins">一日計費上限</param>
        /// <param name="fDayMaxPrice">每日金額上限</param>
        /// <param name="dayBasePrice">基本消費</param>
        /// <param name="FreeMins">免費分鐘數</param>
        /// <param name="GiveMinute">標籤優惠分鐘數</param>
        public CarRentInfo MotoRentMonthComp(DateTime SD, DateTime ED, double priceNmin, double priceHmin, int dayBaseMins, double dayMaxMins
            , List<Holiday> lstHoliday = null, List<MonthlyRentData> mOri = null, int Discount = 0
            , int fDayMaxMins = 0, double fDayMaxPrice = 0, double dayBasePrice = 10, double FreeMins = 0, int GiveMinute = 0
            )
        {
            CarRentInfo re = new CarRentInfo();
            double dre = 0; // 租金
            List<MonthlyRentData> mFinal = new List<MonthlyRentData>(); //剩餘月租點數
            List<string> norDates = new List<string>()  //一般平假日
            {
                eumDateType.wDay.ToString(),
                eumDateType.hDay.ToString()
            };

            if (SD == null || ED == null || SD > ED)
                throw new Exception("SD,ED資料錯誤");

            if (Discount < 0)
                throw new Exception("折扣不可為負數");

            //if (Discount > 0 && Discount < 6)
            //    throw new Exception("折扣不可低於6分鐘");

            double nowDisc = Convert.ToDouble(Discount);    // 使用折抵時數

            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);
            double mins = ED.Subtract(SD).TotalMinutes;     // 使用分鐘數

            if (mOri != null && mOri.Count() > 0)
            {
                if (mOri.Any(x => x.MotoTotalHours < 0 || x.WorkDayRateForMoto < 0 || x.HoildayRateForMoto < 0 || x.MonthlyRentId <= 0))
                    throw new Exception("mOri資料內容錯誤");

                if (mOri.GroupBy(x => x.MonthlyRentId).Where(y => y.Count() > 1).Count() > 0)
                    throw new Exception("MonthlyRentId不可重複");

                mFinal = objUti.Clone(mOri);
            }

            var allDay = GetDateMark(SD, ED, lstHoliday, mOri); //區間內時間註記
            var dayPayList = GetMotoTypeMins(SD, ED, dayBaseMins, dayMaxMins, allDay);  //全分類時間 

            dayPayList = H24FDateSet(dayPayList, dayMaxMins, fDayMaxMins);  //縮減首日及mark首日200=>199

            // 20211213 UPD BY YEH REASON:免費分鐘數減免打開
            // 免費分鐘數減免
            if (FreeMins > 0 && dayPayList != null && dayPayList.Count() > 0)
                dayPayList = befMinsFree(FreeMins, dayPayList);

            re.RentInMins = Convert.ToInt32(dayPayList.Select(x => x.xMins).Sum());

            var dpList = new List<DayPayMins>();//剩餘有分鐘數的

            double UseGiveMinute = 0;
            if (GiveMinute > 0 && mins >= 6)
            {
                double RemainGiveMinute = Convert.ToDouble(GiveMinute);

                dpList = dayPayList.Where(v => v.xMins > 0).OrderByDescending(x => x.xRate).ThenBy(y => y.xSTime).ThenByDescending(z => z.haveNext).ToList();
                dpList.ForEach(x =>
                {
                    if (RemainGiveMinute > 0)
                    {
                        var useDisc = RemainGiveMinute > x.xMins ? x.xMins : RemainGiveMinute;
                        RemainGiveMinute -= useDisc;
                        x.xMins -= useDisc;
                        UseGiveMinute += useDisc;
                        if (useDisc >= 6)
                        {
                            x.UseGiveMinute = useDisc;
                            x.useBaseMins = 6;    // 優惠分鐘數>=6記錄使用基本時間分鐘 不然後續折抵會有問題
                        }
                    }
                });
            }

            #region 判斷每分鐘費率
            if (dayPayList != null && dayPayList.Count() > 0)
            {
                //re.RentInMins = Convert.ToInt32(dayPayList.Select(x => x.xMins).Sum());
                //費率回存
                dayPayList.ForEach(x =>
                {
                    if (x.DateType == eumDateType.wDay.ToString())  // 平日
                        x.xRate = priceNmin;
                    else if (x.DateType == eumDateType.hDay.ToString()) // 假日
                        x.xRate = priceHmin;
                    else if (x.DateType.Contains("h"))
                    {   // 月租假日
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType.Replace("h", string.Empty).Trim()).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.HoildayRateForMoto;
                    }
                    else
                    {   // 月租平日
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.WorkDayRateForMoto;
                    }
                    //20220120 ADD BY ADAM REASON.FLOAT轉DOUBLE會造成小數問題，故取小數點1位四捨五入
                    x.xRate = Math.Round(x.xRate, 1);
                });
                //取GroupId

                dayPayList = GetDateGroup(norDates, "nor_", dayPayList);
            }
            #endregion

            double wDisc = 0; //平日折扣
            double hDisc = 0; //假日則扣  
            double m_wDisc = 0; //平日折扣(所有月租)
            double m_hDisc = 0; //假日則扣(所有月租) 
            var mList = dayPayList.Where(x => !norDates.Any(y => y == x.DateType)).OrderBy(z => z.xSTime).ThenByDescending(w => w.haveNext).ToList();

            //月租內點數先折
            if (mFinal != null && mFinal.Count() > 0)
            {
                foreach (var m in mFinal)
                {
                    var m_disc = Convert.ToDouble(m.MotoTotalHours);//機車月租可折點數
                    var m_wType = m.MonthlyRentId.ToString();
                    var m_hType = m_wType + "h";

                    var m_list = mList.Where(x => x.DateType == m_wType || x.DateType == m_hType).OrderBy(y => y.xSTime).ThenByDescending(w => w.haveNext).ToList();

                    if (m_list != null && m_list.Count() > 0)
                    {
                        if (m_list.Sum(x => x.xMins) > 0)   // 有使用分鐘數才進入計算
                        {
                            if (m_list != null && mList.Count() > 0)
                                m_list.ForEach(x => x.dayGroupId = "mon_" + m.MonthlyRentId.ToString());

                            var mre = MotoRentDiscComp(m.WorkDayRateForMoto, m.HoildayRateForMoto, dayBaseMins, dayBasePrice, ref m_list, m_disc, m_wType, m_hType, fDayMaxMins, fDayMaxPrice, FreeMins);
                            if (mre != null)
                            {
                                dre += mre.Item3;       // 租金
                                m_wDisc += mre.Item1;   // 月租平日折扣
                                m_hDisc += mre.Item2;   // 月租假日則扣
                                m.MotoTotalHours -= Convert.ToSingle(mre.Item1 + mre.Item2);
                            }

                            //還原變動
                            dayPayList.ForEach(x =>
                            {
                                var item = m_list.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
                                if (item != null)
                                    x = item;
                            });
                        }
                    }
                }
            }

            var allMins = dayPayList.Select(x => x.xMins).Sum();
            nowDisc = nowDisc > allMins ? allMins : nowDisc;//自動縮減

            //一般計費日
            var norList = dayPayList.Where(x => norDates.Any(y => y == x.DateType)).ToList();   //一般時段
            if (norList != null && norList.Count() > 0)
            {
                List<string> gIDs = norList.GroupBy(x => x.dayGroupId).Select(y => y.FirstOrDefault().dayGroupId).OrderBy(z => z).ToList();
                if (gIDs != null && gIDs.Count() > 0)
                {
                    foreach (var gID in gIDs)
                    {
                        var gList = norList.Where(x => x.dayGroupId == gID).OrderBy(y => y.xSTime).ThenByDescending(z => z.haveNext).ToList();
                        if (Convert.ToInt32(gList.Sum(x => x.xMins)) > 0)
                        {
                            var gre = MotoRentDiscComp(priceNmin, priceHmin, dayBaseMins, dayBasePrice, ref gList, 0, eumDateType.wDay.ToString(), eumDateType.hDay.ToString(), fDayMaxMins, fDayMaxPrice, FreeMins);
                            if (gre != null)
                            {
                                dre += gre.Item3;
                                wDisc += gre.Item1;
                                hDisc += gre.Item2;
                            }

                            //還原變動
                            dayPayList.ForEach(x =>
                            {
                                var item = gList.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
                                if (item != null)
                                    x = item;
                            });
                        }
                    }
                }
            }

            //價高先折,基本分計價及扣除
            dpList = dayPayList.Where(v => v.xMins > 0).OrderByDescending(x => x.xRate).ThenBy(y => y.xSTime).ThenByDescending(z => z.haveNext).ToList();
            dpList.ForEach(x =>
            {
                if (nowDisc > 0)
                {
                    var useDisc = nowDisc > x.xMins ? x.xMins : nowDisc;
                    //基本分鐘處理
                    if (x.isF24H && x.isStart == 1)
                    {
                        if (x.useBaseMins == 0)
                        {
                            if (useDisc >= dayBaseMins) // 使用折抵 >= 基本分鐘數
                            {
                                if (dayBasePrice > (dayBaseMins * x.xRate))     // 基本消費 >  (基本分鐘 * 每分鐘金額)
                                    dre -= dayBasePrice - (dayBaseMins * x.xRate);    // 租金 = 租金 - 基本消費 - (基本分鐘 * 每分鐘金額)
                                else
                                    dre += (dayBaseMins * x.xRate) - dayBasePrice;      // 租金 = 租金 + (基本分鐘 * 每分鐘金額) - 基本消費

                                x.useBaseMins += dayBaseMins;
                            }
                            else
                            {
                                var f01_over6 = x.xMins - dayBaseMins;  // 使用分鐘 - 基本分鐘
                                if (f01_over6 > 0)
                                    useDisc = useDisc > f01_over6 ? f01_over6 : useDisc;
                            }
                        }
                    }

                    if (useDisc > 0)
                    {
                        if (x.DateType == eumDateType.wDay.ToString())
                            wDisc += useDisc;
                        else if (x.DateType == eumDateType.hDay.ToString())
                            hDisc += useDisc;
                        else if (x.DateType.Contains("h"))
                            hDisc += useDisc;
                        else
                            wDisc += useDisc;
                    }

                    nowDisc -= useDisc;
                    x.xMins -= useDisc;
                    dre -= useDisc * x.xRate;//減去折扣價
                }
            });

            dpList = dpList.Where(x => x.xMins > 0).ToList();
            if (dpList != null && dpList.Count() > 0)
            {
                //分類統計
                var xre = dpList.GroupBy(x => x.DateType).Select(y => new DayPayMins { DateType = y.Key, xMins = y.Select(z => z.xMins).Sum(), xRate = y.Select(w => w.xRate).FirstOrDefault() }).ToList();

                //折扣後租用時數
                re.AfterDiscRentInMins = Convert.ToInt32(xre.Select(x => x.xMins).Sum());
            }

            re.useDisc = Convert.ToInt32(wDisc + hDisc);    //使用一般折扣點數
            re.useMonthDisc = m_wDisc + m_hDisc;    //使用月租折扣點數

            if (mOri != null && mOri.Count() > 0)   //剩餘月租點數
                re.lastMonthDisc = mOri.Select(x => x.MotoTotalHours).Sum() - (m_wDisc + m_hDisc);

            if (mFinal != null && mFinal.Count() > 0)//回傳monthData
            {
                mFinal.ForEach(x =>
                {
                    var mo = mOri.Where(y => y.MonthlyRentId == x.MonthlyRentId).FirstOrDefault();
                    if (mo != null)
                        x.MotoTotalHours = (mo.MotoTotalHours - x.MotoTotalHours);//使用的點數
                });

                mFinal = mFinal.Where(x => x.MotoTotalHours > 0).ToList();//月租點數有使用才回傳
                re.mFinal = mFinal;
            }

            re.useMonthDiscW = m_wDisc;
            re.useMonthDiscH = m_hDisc;
            re.UseGiveMinute = Convert.ToInt32(UseGiveMinute);
            // 20220414 UPD BY YEH REASON:此欄位改為記錄 實際租用時數 - 訂閱制時數 - 標籤優惠時數，用意是記錄一個未折抵的數字，讓APP在部分折抵可以使用
            re.DiscRentInMins = re.RentInMins - Convert.ToInt32(m_wDisc + m_hDisc + UseGiveMinute);

            dre = dre > 0 ? dre : 0;
            re.RentInPay = Convert.ToInt32(Math.Round(dre, 0, MidpointRounding.AwayFromZero));

            return re;
        }
        #endregion

        #region 機車租金計算
        /// <summary>
        /// 機車租金計算(月租平日折扣,月租假日折扣,折扣後金額,月租平日剩餘分,月租假日剩餘分,月租折扣後剩餘分鐘,使用一般折扣點數)
        /// </summary>
        /// <param name="priceNmin">平日價格(分)</param>
        /// <param name="priceHmin">假日價格(分)</param>
        /// <param name="dayBaseMins">基本分鐘數</param>
        /// <param name="dayBasePrice">基本消費</param>
        /// <param name="norList">日期及使用分列表</param>
        /// <param name="Discount">折扣分鐘</param>
        /// <param name="wDateType">平日日期註記</param>
        /// <param name="hDateType">假日日期註記</param>
        /// <param name="fDayMaxMins">首日最大計費分鐘</param>
        /// <param name="fDayMaxPrice">首日最大計費金額</param>
        /// <param name="FreeMinute">免費分鐘</param>
        /// <returns></returns>
        /// <mark>2020-12-21 eason</mark>
        public Tuple<double, double, double, double, double, double, double> MotoRentDiscComp(
            double priceNmin, double priceHmin, double dayBaseMins, double dayBasePrice,
            ref List<DayPayMins> norList, double Discount,
            string wDateType, string hDateType, double fDayMaxMins, double fDayMaxPrice, double FreeMinute = 0
            )
        {
            #region 變數宣告
            var fList = new List<DayPayMins>();//首24H資料
            var norListNoF = new List<DayPayMins>();//一般計費不計首日

            double wDisc = 0; //平日折扣
            double hDisc = 0; //假日則扣  

            double wLastMins = 0;//平日剩餘分
            double hLaseMins = 0;//假日剩餘分

            double nowDisc = 0;//使用折扣
            double norMins = 0;//一般時段計費分鐘
            double norRentInPay = 0;//一般時段租金

            double useDisc = 0;//總使用折扣
            double AfterDiscRentInMins = 0;//折扣後剩餘分鐘          
            #endregion

            nowDisc = Discount;
            if (norList != null && norList.Count() > 0)
            {
                norList = norList.OrderBy(x => x.xSTime).ThenByDescending(y => y.haveNext).ToList();
                fList = norList.Where(x => x.isF24H).OrderByDescending(y => y.haveNext).ToList();

                //費率回存
                norList.ForEach(x =>
                {
                    if (x.DateType == eumDateType.wDay.ToString())  // 平日
                        x.xRate = priceNmin;
                    else if (x.DateType == eumDateType.hDay.ToString()) // 假日
                        x.xRate = priceHmin;
                });

                norMins = norList.Select(x => x.xMins).Sum();       //一般時段總租用分鐘
                nowDisc = nowDisc > norMins ? norMins : nowDisc;    //自動縮減              

                if (fList != null && fList.Count() > 0)
                {
                    var fd = MotoF24HDiscCompute(priceNmin, priceHmin, dayBaseMins, dayBasePrice, fDayMaxPrice, nowDisc, ref fList, wDateType, hDateType, fDayMaxMins, FreeMinute);
                    if (fd != null)
                    {
                        wLastMins += fd.Item4;
                        hLaseMins += fd.Item5;
                        wDisc += fd.Item1;
                        hDisc += fd.Item2;
                        useDisc += fd.Item1 + fd.Item2;
                        nowDisc -= (fd.Item1 + fd.Item2);
                        norRentInPay += fd.Item3;
                        AfterDiscRentInMins += fd.Item4 + fd.Item5;

                        //還原變動
                        norList.ForEach(x =>
                        {
                            var item = fList.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
                            if (item != null)
                                x = item;
                        });
                    }
                }

                //去除首日
                norListNoF = norList.Where(x => !fList.Any(y => y.xSTime == x.xSTime && y.xETime == x.xETime)).ToList();
                if (norListNoF != null && norListNoF.Count() > 0)
                {
                    norListNoF = norListNoF.OrderBy(x => x.xSTime).ThenByDescending(y => y.haveNext).ToList();

                    double wMins = 0;
                    double hMins = 0;
                    double nof_wDisc = 0;
                    double nof_hDisc = 0;
                    var discRe = discCompute(nowDisc, wDateType, hDateType, ref norListNoF);
                    if (discRe != null)
                    {
                        nof_wDisc = discRe.Item1;
                        nof_hDisc = discRe.Item2;
                        wDisc += discRe.Item1;
                        hDisc += discRe.Item2;
                        useDisc += (nof_wDisc + nof_hDisc);
                        nowDisc -= (nof_wDisc + nof_hDisc);

                        //還原變動
                        norList.ForEach(x =>
                        {
                            var item = norListNoF.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
                            if (item != null)
                                x = item;
                        });
                    }

                    norListNoF.ForEach(x =>
                    {
                        wMins += x.DateType == wDateType ? x.xMins : 0;
                        hMins += x.DateType == hDateType ? x.xMins : 0;
                    });

                    wLastMins += wMins;
                    hLaseMins += hMins;

                    norRentInPay += wMins * priceNmin + hMins * priceHmin;
                    AfterDiscRentInMins += wMins + hMins;
                }
            }

            return new Tuple<double, double, double, double, double, double, double>(wDisc, hDisc, norRentInPay, wLastMins, hLaseMins, AfterDiscRentInMins, useDisc);
        }
        #endregion

        #region 租金試算
        /// <summary>
        /// 租金試算-新
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="Price">平日價</param>
        /// <param name="PriceH">假日價</param>
        /// <param name="dayMaxHour">單日時數上限</param>
        /// <param name="lstHoliday">假日清單</param>
        /// <param name="overTime">是否為逾時</param>
        /// <returns></returns>
        /// <mark>eason-2020-11-19</mark>
        public int CarRentCompute(DateTime SD, DateTime ED, int Price, int PriceH, double dayMaxHour, List<Holiday> lstHoliday, bool overTime = false, double baseMinutes = 60)
        {
            int re = 0;
            var minsPro = new MinsProcess(GetCarPayMins);
            var dayPro = overTime ? new DayMinsProcess(CarOverTimeMinsToPayMins) : null;

            double wPriceHour = 0; //平日每小時價格
            double hPriceHour = 0; //假日每小時價格

            if (overTime)
            {//逾時超過6小時以10小時價格計算
                wPriceHour = Price / 10;
                hPriceHour = PriceH / 10;
            }
            else
            {
                wPriceHour = Price / dayMaxHour;
                hPriceHour = PriceH / dayMaxHour;
            }

            var result = GetRangeMins(SD, ED, baseMinutes, dayMaxHour * 60, lstHoliday, minsPro, dayPro);

            if (result != null)
            {
                double tPrice = 0;
                tPrice += Math.Floor((result.Item1 / 60) * wPriceHour);
                tPrice += Math.Floor((result.Item2 / 60) * hPriceHour);

                if (tPrice > 0)
                    re = Convert.ToInt32(tPrice);
            }
            return re;
        }
        #endregion

        #region 預估里程試算
        /// <summary>
        /// 預估里程試算
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="MilageBase">專案的每公里金額</param>
        /// <param name="MilageDef">預設每公里費用</param>
        /// <param name="baseMil">每小時幾公里</param>
        /// <param name="lstHoliday">假日清單</param>
        /// <returns></returns>
        public int CarMilageCompute(DateTime SD, DateTime ED, float MilageBase, float MilageDef, float baseMil, List<Holiday> lstHoliday)
        {
            int MilagePrice = 0;

            var AllMinute = GetCarRangeMins(SD, ED, 60, 600, lstHoliday);  //基本分鐘數及單日分鐘上限寫死
            var TotalHour = (AllMinute.Item1 + AllMinute.Item2) / 60;

            if (MilageBase < 0)
            {
                MilagePrice = Convert.ToInt32(Math.Round((TotalHour * baseMil) * MilageDef, 0, MidpointRounding.AwayFromZero));
            }
            else
            {
                MilagePrice = Convert.ToInt32(Math.Round((TotalHour * baseMil) * MilageBase, 0, MidpointRounding.AwayFromZero));
            }
            return MilagePrice;
        }
        #endregion

        #region 汽車每個DataType的List
        /// <summary>
        /// 汽車每個DataType的List,不含逾時
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘數</param>
        /// <param name="dayMaxMins">日最大計費分鐘數</param>
        /// <param name="markDays">標記日期</param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> GetCarTypeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<DayPayMins> markDays)
        {
            var minsPro = new MinsProcess(GetCarPayMins);
            return GetTypeMins(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro);
        }
        #endregion

        #region 機車全分類時間
        /// <summary>
        /// 機車全分類時間
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">日基本分鐘</param>
        /// <param name="dayMaxMins">日最大分鐘</param>
        /// <param name="markDays">日期列表</param>
        /// <returns></returns>
        /// <mark>2020-12-21 eason</mark>
        public List<DayPayMins> GetMotoTypeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<DayPayMins> markDays)
        {
            var minsPro = new MinsProcess(GetMotoPayMins);
            return GetTypeMins(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro);
        }
        #endregion

        #region 所有DataType的List
        /// <summary>
        /// 所有DataType的List
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘數</param>
        /// <param name="dayMaxMins">日最大計費分鐘數</param>
        /// <param name="markDays">標記日期</param>
        /// <param name="minsPro">未滿60分處理</param>
        /// <param name="dayPro">單日特殊邏輯</param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> GetTypeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<DayPayMins> markDays, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
        {
            List<DayPayMins> re = new List<DayPayMins>();

            if (markDays != null && markDays.Count() > 0)
                markDays = FillDate(SD, ED, markDays);

            var res = GetTypeDayFlow(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro, dayPro);

            if (res != null && res.Count() > 0)
                re = res.OrderBy(x => x.xDate).ToList();

            return re;
        }
        #endregion

        #region 取得List中所有DateType的計費時間
        /// <summary>
        /// 取得List中所有DateType的計費時間
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本計費分鐘</param>
        /// <param name="dayMaxMins">每日計費分鐘上限</param>
        /// <param name="markDays">日期分類列表</param>
        /// <param name="minsPro">未滿60加工</param>
        /// <param name="dayPro">每個計費日特殊邏輯</param>
        /// <returns></returns>
        /// <mark>2020-12-02 eason</mark>
        public List<DayPayMins> GetTypeDayFlow(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<DayPayMins> markDays, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
        {
            List<DayPayMins> re = new List<DayPayMins>();

            if (SD == null && ED == null && SD > ED)
                throw new Exception("SD,ED不可為null");

            if (SD > ED)
                throw new Exception("起日不可大於迄日");

            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);

            string str_sd = SD.ToString("yyyyMMdd");
            string str_ed = ED.ToString("yyyyMMdd");

            string type_sd = markDays.Where(x => x.xDate == str_sd).Select(y => y.DateType).FirstOrDefault();
            string type_ed = markDays.Where(x => x.xDate == str_ed).Select(y => y.DateType).FirstOrDefault();

            if (SD.Date == ED.Date)
            {
                var xre = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, new List<Holiday>(), minsPro, dayPro);
                if (xre != null && xre.Count > 0)
                {
                    xre.ForEach(x => x.DateType = type_sd);
                    re.AddRange(xre);
                }
            }
            else if (SD.AddDays(1) >= ED)
            {
                if (type_sd == type_ed)
                {
                    var xre = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, new List<Holiday>(), minsPro, dayPro);
                    if (xre != null && xre.Count > 0)
                    {
                        xre.ForEach(x => x.DateType = type_sd);
                        re.AddRange(xre);
                    }
                }
                else
                {
                    var fDays = markDays.Where(x => x.DateType == type_ed).ToList();
                    var iDays = FromDayPayMins(fDays);
                    var xre = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, iDays, minsPro, dayPro);
                    if (xre != null && xre.Count() > 0)
                    {
                        xre.ForEach(x => { x.DateType = x.isMarkDay ? type_ed : type_sd; });
                        re.AddRange(xre);
                    }
                }
            }
            else
            {
                while (SD < ED)
                {
                    var sd24 = SD.AddHours(24);

                    string typ_sd = markDays.Where(x => x.xDate == SD.ToString("yyyyMMdd")).Select(y => y.DateType).FirstOrDefault();
                    string typ_ed = markDays.Where(x => x.xDate == ED.ToString("yyyyMMdd")).Select(y => y.DateType).FirstOrDefault();
                    string typ_sd24 = markDays.Where(x => x.xDate == sd24.ToString("yyyyMMdd")).Select(y => y.DateType).FirstOrDefault();

                    if (ED > sd24)
                    {
                        if (typ_sd == typ_sd24)
                        {
                            var re24 = GetH24DayPayList(SD, sd24, baseMinutes, dayMaxMins, new List<Holiday>(), minsPro, dayPro);
                            if (re24 != null && re24.Count() > 0)
                            {
                                re24.ForEach(x => x.DateType = typ_sd);
                                re.AddRange(re24);
                            }
                        }
                        else
                        {
                            var fDays = markDays.Where(x => x.DateType == typ_sd24).ToList();
                            var iDays = FromDayPayMins(fDays);
                            var re24 = GetH24DayPayList(SD, sd24, baseMinutes, dayMaxMins, iDays, minsPro, dayPro);
                            if (re24 != null && re24.Count() > 0)
                            {
                                re24.ForEach(x => { x.DateType = x.isMarkDay ? typ_sd24 : typ_sd; });
                                re.AddRange(re24);
                            }
                        }
                    }
                    else
                    {
                        if (typ_sd == typ_ed)
                        {
                            var reLast = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, new List<Holiday>(), minsPro, dayPro);
                            if (reLast != null && reLast.Count() > 0)
                            {
                                reLast.ForEach(x => x.DateType = typ_sd);
                                re.AddRange(reLast);
                            }
                        }
                        else
                        {
                            var fDays = markDays.Where(x => x.DateType == typ_ed).ToList();
                            var iDays = FromDayPayMins(fDays);
                            var reLast = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, iDays, minsPro, dayPro);
                            if (reLast != null && reLast.Count() > 0)
                            {
                                reLast.ForEach(x => { x.DateType = x.isMarkDay ? typ_ed : typ_sd; });
                                re.AddRange(reLast);
                            }
                        }
                    }
                    SD = sd24;
                }
            }

            return re;
        }
        #endregion

        #region 取得單日時間軸轉註記日計費時數列表
        /// <summary>
        /// 取得單日時間軸轉註記日計費時數列表,不可大於24小時
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘</param>
        /// <param name="dayMaxMins">單日計費分鐘上限</param>
        /// <param name="lstHoliday">註記日</param>
        /// <param name="minsPro">未滿60分處理</param>
        /// <param name="dayPro">單日特殊邏輯</param>
        /// <returns></returns>
        /// <mark>2020-12-02 eason</mark>
        public List<DayPayMins> GetH24DayPayList(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
        {
            List<DayPayMins> re = new List<DayPayMins>();

            string str_sd = SD.ToString("yyyyMMdd");
            string str_ed = ED.ToString("yyyyMMdd");

            bool sd_isMarkDay = lstHoliday.Any(x => x.HolidayDate == str_sd);
            bool ed_isMarkDay = lstHoliday.Any(x => x.HolidayDate == str_ed);

            var re24 = GetH24Mins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);

            if (re24.Item1 == 0 || re24.Item2 == 0)//都是無註記,或都是註記日
            {
                DayPayMins item = new DayPayMins()
                {
                    isMarkDay = sd_isMarkDay,
                    xDate = str_sd,
                    xSTime = SD.ToString("yyyyMMddHHmm"),
                    xETime = ED.ToString("yyyyMMddHHmm"),
                    xMins = sd_isMarkDay ? re24.Item2 : re24.Item1,
                    haveNext = 0,
                    isStart = 1,
                    isEnd = 1,
                    isFull24H = true
                };
                re.Add(item);
            }
            else//同時有無註記,註記日
            {
                //首日
                DayPayMins item_sd = new DayPayMins()
                {
                    isMarkDay = sd_isMarkDay,
                    xDate = str_sd,
                    xSTime = SD.ToString("yyyyMMddHHmm"),
                    xETime = ED.ToString("yyyyMMddHHmm"),
                    xMins = sd_isMarkDay ? re24.Item2 : re24.Item1,
                    haveNext = 1,
                    isStart = 1,
                    isEnd = 0,
                    isFull24H = false
                };
                re.Add(item_sd);

                //隔日
                DayPayMins item_ed = new DayPayMins()
                {
                    isMarkDay = ed_isMarkDay,
                    xDate = str_ed,
                    xSTime = SD.ToString("yyyyMMddHHmm"),
                    xETime = ED.ToString("yyyyMMddHHmm"),
                    xMins = ed_isMarkDay ? re24.Item2 : re24.Item1,
                    haveNext = 0,
                    isStart = 0,
                    isEnd = 1,
                    isFull24H = false
                };
                re.Add(item_ed);
            }

            return re;
        }
        #endregion

        #region 時間區段內平日計費分鐘總和,假日計費分鐘總和
        /// <summary>
        /// 時間區段內平日計費分鐘總和,假日計費分鐘總和
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="dayMaxMins">單日分鐘上限</param>
        /// <param name="lstHoliday">假日清單</param>
        /// <returns>平日計費分鐘,假日計費分鐘</returns>
        /// <mark>eason-2020-11-19</mark>
        public Tuple<double, double> GetRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
        {
            double n_allMins = 0;//總平日分鐘
            double h_allMins = 0;//總假日分鐘

            if (SD == null && ED == null && SD > ED)
                throw new Exception("SD,ED不可為null");

            if (SD > ED)
                throw new Exception("起日不可大於迄日");

            if (SD.Date == ED.Date || SD.AddDays(1) >= ED)
                return GetH24Mins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
            else
            {
                while (SD < ED)
                {
                    var sd24 = SD.AddHours(24);
                    if (ED > sd24)
                    {
                        var re24 = GetH24Mins(SD, sd24, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
                        if (re24 != null)
                        {
                            n_allMins += re24.Item1;
                            h_allMins += re24.Item2;
                        }
                    }
                    else
                    {
                        var reLast = GetH24Mins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
                        if (reLast != null)
                        {
                            n_allMins += reLast.Item1;
                            h_allMins += reLast.Item2;
                        }
                    }
                    SD = sd24;
                }
            }

            return new Tuple<double, double>(n_allMins, h_allMins);
        }
        #endregion

        #region 汽車未超時
        /// <summary>
        /// 汽車未超時
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="baseMinutes"></param>
        /// <param name="dayMaxMins"></param>
        /// <param name="lstHoliday"></param>
        /// <returns></returns>
        public Tuple<double, double> GetCarRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetCarPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);
        }
        #endregion

        #region 機車未超時
        /// <summary>
        /// 機車未超時
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="baseMinutes"></param>
        /// <param name="dayMaxMins"></param>
        /// <param name="lstHoliday"></param>
        /// <returns></returns>
        public Tuple<double, double> GetMotoRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetMotoPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);
        }
        #endregion

        #region 汽車超時時間-計算用
        /// <summary>
        /// 汽車超時時間-計算用
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="baseMinutes"></param>
        /// <param name="dayMaxMins"></param>
        /// <param name="lstHoliday"></param>
        /// <returns></returns>
        /// <remarks>2020-12-03 eason</remarks>
        public Tuple<double, double> GetCarOutComputeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetCarPayMins);
            var dayPro = new DayMinsProcess(CarOverTimeMinsToPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
        }
        #endregion

        #region 汽車未滿1小時分鐘改為計費分鐘
        /// <summary>
        /// 汽車未滿1小時分鐘改為計費分鐘
        /// </summary>
        /// <param name="Mins"></param>
        /// <returns></returns>
        /// <mark>2020-11-19 eason</mark>
        public double GetCarPayMins(double Mins)
        {
            double re = 0;

            if (Mins < 0)
                throw new Exception("不可負數");
            if (Mins > 60)
                throw new Exception("不可大於60分鐘");

            if (Mins >= 15 && Mins < 45)
                re = 30;
            else if (Mins >= 45)
                re = 60;

            return re;
        }
        #endregion

        #region 機車未滿1小時分鐘改為計費分鐘
        /// <summary>
        /// 機車未滿1小時分鐘改為計費分鐘
        /// </summary>
        /// <param name="Mins"></param>
        /// <returns></returns>
        public double GetMotoPayMins(double Mins)
        {
            //double re = 0;

            //if (Mins > 0 && Mins < 6)
            //    re = 6;
            //else
            //    re = Mins;

            //return re;
            return Mins;
        }
        #endregion

        #region 汽車逾時單日時間轉計費時間
        /// <summary>
        /// 汽車逾時單日時間轉計費時間
        /// </summary>
        /// <param name="wMins"></param>
        /// <param name="hMins"></param>
        /// <returns></returns>
        /// <mark>2020-11-19 eason</mark>
        public void CarOverTimeMinsToPayMins(ref double wMins, ref double hMins)
        {//逾時超過6小時當作10小時
            if (wMins >= 360)
                wMins = 600;
            if (hMins >= 360)
                hMins = 600;
        }
        #endregion

        #region 24小時計費分鐘數
        /// <summary>
        /// 24小時計費分鐘數
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <param name="minsPro">未滿60分鐘轉計費分鐘</param>
        /// <returns>平日計費分鐘,假日計費分鐘</returns>
        /// <mark>起迄不可超過24小時</mark>
        /// <mark>eason-2020-11-19</mark>
        public Tuple<double, double> GetH24Mins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
        {
            double n_allMins = 0;//總平日分鐘
            double h_allMins = 0;//總假日分鐘

            if (SD == null || ED == null)
                throw new Exception("SD,ED不可為null");

            if (SD > ED)
                throw new Exception("起日不可大於迄日");

            SD = SD.AddSeconds(SD.Second * -1); //去秒數
            ED = ED.AddSeconds(ED.Second * -1); //去秒數

            double mins = ED.Subtract(SD).TotalMinutes;
            if (mins > (24 * 60))
                throw new Exception("不可大於24小時");

            var sd_end = Convert.ToDateTime(SD.AddDays(1).ToString("yyyy/MM/dd 00:00:00"));//首日結束時間

            double xhours = Math.Floor(mins / 60);
            double xmins = mins % 60;//未達1小時分鐘數

            var sd10 = SD.AddMinutes(dayMaxMins);//計費截止時間

            string str_sd = SD.ToString("yyyyMMdd");
            string str_ed = ED.ToString("yyyyMMdd");

            bool sd_isHoliday = lstHoliday.Any(x => x.HolidayDate == str_sd);
            bool ed_isHoliday = lstHoliday.Any(x => x.HolidayDate == str_ed);

            if (mins < baseMinutes)//未達基本分鐘
            {
                if (sd_isHoliday)
                    h_allMins = baseMinutes;
                else
                    n_allMins = baseMinutes;
            }
            else
            {
                if (SD.Date == ED.Date)
                {
                    if (minsPro != null && xmins > 0)
                        xmins = minsPro(xmins);

                    var allPayMins = xhours * 60 + xmins;
                    allPayMins = allPayMins > dayMaxMins ? dayMaxMins : allPayMins;

                    if (sd_isHoliday)
                        h_allMins = allPayMins;
                    else
                        n_allMins = allPayMins;
                }
                else
                {
                    if (mins < dayMaxMins)//未達上限
                        sd10 = ED;

                    if (sd10 <= sd_end)//計費未跨日
                    {
                        var pay_mins = sd10.Subtract(SD).TotalMinutes;
                        var pay_xhours = Math.Floor(pay_mins / 60);
                        var pay_xmins = pay_mins % 60;

                        if (minsPro != null && pay_xmins > 0 && pay_xmins < 60)
                            pay_xmins = minsPro(pay_xmins);

                        var allPayMis = pay_xhours * 60 + pay_xmins;

                        if (sd_isHoliday)
                            h_allMins = allPayMis;
                        else
                            n_allMins = allPayMis;
                    }
                    else//跨日
                    {
                        var bef_mins = sd_end.Subtract(SD).TotalMinutes;//前日總分鐘
                        var bef_xhours = Math.Floor(Convert.ToDouble(bef_mins) / 60);//前日小時
                        var bef_xmins = bef_mins % 60;//前日分

                        if (bef_xmins == 0)//物理整點
                        {
                            if (sd_isHoliday)
                                h_allMins += bef_xhours * 60;
                            else
                                n_allMins += bef_xhours * 60;

                            double _af_mins = sd10.Subtract(sd_end).TotalMinutes;
                            double _af_xhour = Math.Floor(_af_mins / 60);//後日小時
                            double _af_xmins = _af_mins % 60;//後日分

                            if (minsPro != null && _af_xmins > 0 && _af_xmins < 60)//未滿60處理
                                _af_xmins = minsPro(_af_xmins);

                            var allPayMins = _af_xhour * 60 + _af_xmins;

                            if (ed_isHoliday)
                                h_allMins += allPayMins;
                            else
                                n_allMins += allPayMins;
                        }
                        else
                        {
                            //前日完整hour
                            if (sd_isHoliday)
                                h_allMins += bef_xhours * 60;
                            else
                                n_allMins += bef_xhours * 60;

                            //後日-前日相對整點的point
                            var lastMins = ED.Subtract(SD.AddHours(bef_xhours)).TotalMinutes;

                            if (lastMins < 60)
                            {
                                if (minsPro != null && lastMins > 0 && lastMins < 60)
                                    lastMins = minsPro(lastMins);

                                if (sd_isHoliday)
                                    h_allMins += lastMins;
                                else
                                    n_allMins += lastMins;
                            }
                            else
                            {
                                //交界小時算前日
                                if (sd_isHoliday)
                                    h_allMins += 60;
                                else
                                    n_allMins += 60;

                                //後日-相對整點起
                                var af_star = SD.AddHours(bef_xhours + 1);
                                var af_mins = sd10.Subtract(af_star).TotalMinutes;
                                var af_xhours = Math.Floor(af_mins / 60);
                                var af_xmins = af_mins % 60;

                                if (minsPro != null && af_xmins > 0 && af_xmins < 60)//尾數分轉有計費分鐘
                                    af_xmins = minsPro(af_xmins);

                                if (ed_isHoliday)
                                    h_allMins += af_xhours * 60 + af_xmins;
                                else
                                    n_allMins += af_xhours * 60 + af_xmins;
                            }
                        }
                    }
                }
            }

            if (dayPro != null)//日計費分鐘特殊邏輯
                dayPro(ref n_allMins, ref h_allMins);

            return new Tuple<double, double>(n_allMins, h_allMins);
        }
        #endregion

        #region 判斷是否屬於假日
        /// <summary>
        /// 判斷是否屬於假日
        /// </summary>
        /// <param name="holidayList"></param>
        /// <param name="checkDateTime"></param>
        /// <returns></returns>
        public bool IsInHoliday(List<Holiday> holidayList, DateTime checkDateTime)
        {
            foreach (Holiday holiday in holidayList)
            {
                string tempDate = holiday.HolidayDate.Trim();
                DateTime temp;

                if (DateTime.TryParseExact(tempDate, "yyyyMMdd", null, DateTimeStyles.None, out temp))
                {
                    if (temp.Date == checkDateTime.Date)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region 判斷某日期是否在日期區間內
        /// <summary> 
        /// 判斷某日期是否在日期區間內
        /// </summary> 
        /// <param name="dt_keyin">要判斷的日期</param> 
        /// <param name="dt_start">開始日期</param> 
        /// <param name="dt_end">結束日期</param> 
        /// <returns></returns>  
        private bool IsInDate(DateTime dt_keyin, DateTime dt_start, DateTime dt_end)
        {
            return dt_keyin.CompareTo(dt_start) >= 0 && dt_keyin.CompareTo(dt_end) <= 0;
        }
        #endregion

        #region 區間折扣計算
        /// <summary>
        /// 區間折扣計算
        /// </summary>
        /// <param name="disc"></param>
        /// <param name="wDay">平日DateType</param>
        /// <param name="hDay">假日DateType</param>
        /// <param name="norList">折扣日期列表,須包含DateType, xDate, xMins</param>
        /// <returns>平日折扣, 假日則扣, 最大可折抵分鐘</returns>
        /// <mark>2020-12-09 eason</mark>
        public Tuple<double, double, double> discCompute(double disc, string wDay, string hDay, ref List<DayPayMins> norList)
        {
            double wDisc = 0;//平日折扣
            double hDisc = 0;//假日則扣
            double nowDisc = 0;//現在點數
            double DiscRentInMins = 0;//最大可折抵分鐘

            if (norList != null && norList.Count() > 0 && disc > 0)
            {
                if (norList.Any(x => string.IsNullOrEmpty(x.DateType) || string.IsNullOrWhiteSpace(x.DateType)))
                    throw new Exception("discCompute : DateType為必填");

                if (norList.Any(x => x.xRate <= 0))
                    throw new Exception("discCompute : xRate為必填");

                nowDisc = disc;
                norList = norList.OrderByDescending(x => x.xRate).ThenBy(y => y.xDate).ToList();//價高先折
                double mins = norList.Select(x => x.xMins).Sum();

                if (nowDisc > mins)//自動縮減
                    nowDisc = mins;//實際使用點數
                DiscRentInMins = mins;//最大可折抵分鐘

                norList.ForEach(x =>
                {
                    if (nowDisc > 0)
                    {
                        if (nowDisc >= x.xMins)
                        {
                            wDisc += x.DateType == wDay ? x.xMins : 0;
                            hDisc += x.DateType == hDay ? x.xMins : 0;
                            nowDisc -= x.xMins;
                            x.xMins = 0;
                        }
                        else
                        {
                            wDisc += x.DateType == wDay ? nowDisc : 0;
                            hDisc += x.DateType == hDay ? nowDisc : 0;
                            x.xMins -= nowDisc;
                            nowDisc = 0;
                        }
                    }
                });

            }

            return new Tuple<double, double, double>(wDisc, hDisc, DiscRentInMins);
        }
        #endregion

        #region 前n分鐘0元
        /// <summary>
        /// 前n分鐘0元
        /// </summary>
        /// <param name="disc">前n分鐘</param>
        /// <param name="sour">sour</param>
        /// <returns></returns>
        public List<DayPayMins> befMinsFree(double disc, List<DayPayMins> sour)
        {
            double lastDisc = 0;//現在點數

            if (sour != null && sour.Count() > 0)
            {
                lastDisc = disc;
                var re = objUti.Clone(sour);
                re = re.OrderBy(x => x.xDate).ThenBy(y => y.xSTime).ThenByDescending(z => z.haveNext).ToList();
                double mins = re.Select(x => x.xMins).Sum();

                if (lastDisc > mins)//自動縮減
                    lastDisc = mins;//實際使用點數

                foreach (var x in re)
                {
                    if (lastDisc > 0)
                    {
                        var useDisc = lastDisc > x.xMins ? x.xMins : lastDisc;
                        lastDisc -= useDisc;
                        x.xMins = x.xMins - useDisc;
                    }
                }

                return re;
            }
            else
                return sour;
        }
        #endregion

        #region 首24H折扣後
        /// <summary>
        /// 首24H折扣後(平日折扣,假日折扣,折扣後金額,平日剩餘分,假日剩餘分)
        /// </summary>
        /// <param name="priceNmin">平日價格(分)</param>
        /// <param name="priceHmin">假日價格(分)</param>
        /// <param name="dayBaseMins">基本分鐘數</param>
        /// <param name="dayBasePrice">基本消費</param>
        /// <param name="dayMaxPrice">單日上限金額</param>
        /// <param name="Discount">折扣</param>
        /// <param name="fList">首日列表</param>
        /// <param name="wDateType">平日DateType</param>
        /// <param name="hDateType">假日DateType</param>
        /// <param name="fDayMaxMins">單日上限分鐘</param>
        /// <param name="FreeMinute">免費分鐘</param>
        /// <returns></returns>
        public Tuple<double, double, double, double, double> MotoF24HDiscCompute(
            double priceNmin, double priceHmin, double dayBaseMins, double dayBasePrice, double dayMaxPrice, double Discount,
            ref List<DayPayMins> fList, string wDateType, string hDateType, double fDayMaxMins, double FreeMinute = 0
            )
        {
            string funNm = "MotoF24HDiscCompute : ";
            double f_wDisc = 0; //f1w+f2w
            double f_hDisc = 0; //f1h+f2h
            double f1_wDisc = 0;//H24日1平日折扣
            double f1_hDisc = 0;//H24日1假日折扣
            double f2_wDisc = 0;//H24日2平日折扣
            double f2_hDisc = 0;//H24日2假日折扣
            double f24Pay = 0;//首24租金
            double UseDisc = 0;//使用折扣
            double Mins = 0;//一般時段計費分鐘
            double wLastMins = 0;//折扣後平日剩餘分鐘
            double hLastMins = 0;//折扣後假日剩餘分鐘

            if (fList.Any(x => !x.isF24H))
                throw new Exception(funNm + "不可傳入非首日資料");

            UseDisc = Discount;
            if (fList != null && fList.Count() > 0)
            {
                fList = fList.OrderBy(x => x.xDate).ThenByDescending(y => y.haveNext).ToList();
                Mins = fList.Select(x => x.xMins).Sum();
                UseDisc = UseDisc > Mins ? Mins : UseDisc;//自動縮減      
                UseDisc = UseDisc > fDayMaxMins ? fDayMaxMins : UseDisc;//對大分鐘縮減
                double tmpUseDisc = UseDisc;//首日折扣

                string sAll = "all";
                string sFir = "first";
                string sLas = "last";

                var fdate = fList.FirstOrDefault();//取出首日
                string FdateType = "";//首日資料狀態

                if (fdate.isFull24H || (!fdate.isFull24H && fList.Count() == 2))
                    FdateType = sAll;
                else if (fdate.haveNext == 1 && fList.Count() == 1)
                    FdateType = sFir;
                else if (fdate.haveNext == 0 && fList.Count() == 1)
                    FdateType = sLas;
                else
                    throw new Exception(funNm + "fList資料錯誤");

                if (FdateType == sAll)
                {
                    var useDisc01 = tmpUseDisc > fdate.xMins ? fdate.xMins : tmpUseDisc;    //折扣自動縮減
                    double f01_over6 = 0;    //超過基本分鐘的時數

                    if (FreeMinute == 0)  // 免費分鐘數等於0，才計算超過基本分鐘的時數
                    {
                        if (fdate.useBaseMins == 0) // 使用基本時間分鐘=0，才計算超過基本分鐘的時數
                        {
                            f01_over6 = fdate.xMins > dayBaseMins ? (fdate.xMins - dayBaseMins) : 0;
                        }
                        else
                        {
                            f01_over6 = fdate.xMins;
                        }
                    }
                    else
                    {
                        f01_over6 = fdate.xMins;
                    }

                    if (fdate.DateType == wDateType)    // 平日
                    {
                        #region 平日
                        if (useDisc01 == 0) // 折抵分鐘數 = 0
                        {
                            wLastMins += fdate.xMins;
                            if (FreeMinute == 0)    // 免費分鐘數 = 0
                            {
                                if (fdate.UseGiveMinute == 0)     // 使用優惠分鐘數 = 0
                                {
                                    f24Pay += (fdate.xMins - dayBaseMins) * priceNmin + dayBasePrice;   // 租金 = (使用分鐘數-基本分鐘數) * 每分鐘價格 + 基本消費
                                }
                                else
                                {
                                    f24Pay += fdate.xMins * priceNmin;  // 租金 = 使用分鐘數 * 每分鐘價格
                                }
                            }
                            else
                            {
                                f24Pay += fdate.xMins * priceNmin + dayBasePrice;   // 租金 = 使用分鐘數 * 每分鐘價格 + 基本消費
                            }
                        }
                        else
                        {
                            if (fdate.UseGiveMinute == 0)   // 使用優惠分鐘數 = 0
                            {
                                if (useDisc01 >= dayBaseMins)   // 折抵分鐘數 >= 基本分鐘數
                                {
                                    wLastMins += fdate.xMins - useDisc01; // 折扣後平日剩餘分鐘 = 使用分鐘數 - 折抵分鐘數
                                    f24Pay += (fdate.xMins - useDisc01) * priceNmin;    // 租金 = (使用分鐘數-折抵分鐘數) * 每分鐘價格
                                    fdate.xMins -= useDisc01;   // 使用分鐘數 = 使用分鐘數 - 折抵分鐘數
                                    fdate.useBaseMins = dayBaseMins;
                                }
                                else
                                {
                                    //折扣小於基本分只能折扣超過基本分的時數
                                    if (f01_over6 > 0)
                                    {
                                        useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                        // 20211213 UPD BY YEH REASON:有訂閱制且扣除免費分鐘後使用時間<基本分鐘就會進來，改為不扣除基本分鐘數下去計算
                                        f24Pay += dayBasePrice + (f01_over6 - useDisc01) * priceNmin;
                                        wLastMins += fdate.xMins - useDisc01;
                                        fdate.xMins -= useDisc01;
                                    }
                                    else
                                    {
                                        useDisc01 = 0;
                                        f24Pay += dayBasePrice;//使用未超過基本分,且折扣小於基本分時不能折
                                        wLastMins += fdate.xMins - useDisc01;
                                        fdate.xMins -= useDisc01;
                                    }
                                }
                            }
                            else
                            {
                                if (f01_over6 > 0)
                                {
                                    useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                    f24Pay += (f01_over6 - useDisc01) * priceNmin;
                                    wLastMins += fdate.xMins - useDisc01;
                                    fdate.xMins -= useDisc01;
                                }
                            }
                            tmpUseDisc -= useDisc01;
                            f1_wDisc += useDisc01;
                        }
                        #endregion
                    }
                    else if (fdate.DateType == hDateType)   // 假日
                    {
                        #region 假日
                        if (useDisc01 == 0) // 折抵分鐘數 = 0
                        {
                            hLastMins += fdate.xMins;
                            if (FreeMinute == 0)
                            {
                                if (fdate.UseGiveMinute == 0)     // 使用優惠分鐘數 = 0
                                {
                                    f24Pay += (fdate.xMins - dayBaseMins) * priceHmin + dayBasePrice;   // 租金 = (使用分鐘數-基本分鐘數) * 每分鐘價格 + 基本消費
                                }
                                else
                                {
                                    f24Pay += fdate.xMins * priceHmin;  // 租金 = 使用分鐘數 * 每分鐘價格
                                }
                            }
                            else
                                f24Pay += fdate.xMins * priceHmin + dayBasePrice;   // 租金 = 使用分鐘數 * 每分鐘價格 + 基本消費
                        }
                        else
                        {
                            if (fdate.UseGiveMinute == 0)   // 使用優惠分鐘數 = 0
                            {
                                if (useDisc01 >= dayBaseMins)   // 折抵分鐘數 >= 基本分鐘數
                                {
                                    hLastMins += (fdate.xMins - useDisc01); // 折扣後平日剩餘分鐘 = 使用分鐘數 - 折抵分鐘數
                                    f24Pay += (fdate.xMins - useDisc01) * priceHmin;    // 租金 = (使用分鐘數-折抵分鐘數) * 每分鐘價格
                                    fdate.xMins -= useDisc01;   // 使用分鐘數 = 使用分鐘數 - 折抵分鐘數
                                    fdate.useBaseMins = dayBaseMins;
                                }
                                else
                                {
                                    if (f01_over6 > 0)
                                    {
                                        useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                        //折扣小於基本分只能折扣超過基本分的部分
                                        // 20211213 UPD BY YEH REASON:有訂閱制且扣除免費分鐘後使用時間<基本分鐘就會進來，改為不扣除基本分鐘數下去計算
                                        f24Pay += dayBasePrice + (f01_over6 - useDisc01) * priceHmin;
                                        hLastMins += (fdate.xMins - useDisc01);
                                        fdate.xMins -= useDisc01;
                                    }
                                    else
                                    {
                                        useDisc01 = 0;
                                        f24Pay += dayBasePrice;//使用未超過基本分,且折扣小於基本分時不能折
                                        wLastMins += fdate.xMins - useDisc01;
                                        fdate.xMins -= useDisc01;
                                    }
                                }
                            }
                            else
                            {
                                if (f01_over6 > 0)
                                {
                                    useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                    f24Pay += (f01_over6 - useDisc01) * priceHmin;
                                    hLastMins += (fdate.xMins - useDisc01);
                                    fdate.xMins -= useDisc01;
                                }
                            }
                            tmpUseDisc -= useDisc01;
                            f1_hDisc += useDisc01;
                        }
                        #endregion
                    }

                    if (fdate.haveNext == 1 && fList != null && fList.Count() >= 2)
                    {
                        var fdate02 = fList.LastOrDefault();
                        double useDisc02 = 0;
                        if (tmpUseDisc > 0)
                            useDisc02 = tmpUseDisc > fdate02.xMins ? fdate02.xMins : tmpUseDisc;

                        if (fdate02.DateType == wDateType)
                        {
                            tmpUseDisc -= useDisc02;
                            f2_wDisc += useDisc02;
                            f24Pay += (fdate02.xMins - useDisc02) * priceNmin;
                            wLastMins += (fdate02.xMins - useDisc02);
                            fdate02.xMins -= useDisc02;
                        }
                        else if (fdate02.DateType == hDateType)
                        {
                            tmpUseDisc -= useDisc02;
                            f2_hDisc += useDisc02;
                            f24Pay += (fdate02.xMins - useDisc02) * priceHmin;
                            hLastMins += (fdate02.xMins - useDisc02);
                            fdate02.xMins -= useDisc02;
                        }
                    }

                    f_wDisc = f1_wDisc + f2_wDisc;
                    f_hDisc = f1_hDisc + f2_hDisc;
                }
                else if (FdateType == sFir)
                {
                    var useDisc = tmpUseDisc > fdate.xMins ? fdate.xMins : tmpUseDisc;//折扣自動縮減
                    var f01_over6 = fdate.xMins > dayBaseMins ? (fdate.xMins - dayBaseMins) : 0;//超過基本分鐘的部分                    

                    if (fdate.DateType == wDateType)
                    {
                        if (useDisc == 0)
                        {
                            wLastMins += fdate.xMins;
                            f24Pay += (fdate.xMins - dayBaseMins) * priceNmin + dayBasePrice;
                        }
                        else
                        {
                            if (useDisc >= dayBaseMins)
                            {
                                wLastMins += (fdate.xMins - useDisc);
                                f24Pay += (fdate.xMins - useDisc) * priceNmin;
                                fdate.xMins -= useDisc;
                                fdate.useBaseMins = dayBaseMins;
                            }
                            else
                            {
                                //折扣小於基本分只能折扣超過基本分的部分
                                if (f01_over6 > 0)
                                {
                                    useDisc = useDisc > f01_over6 ? f01_over6 : useDisc;
                                    f24Pay += dayBasePrice + ((fdate.xMins - dayBaseMins) - useDisc) * priceNmin;
                                    wLastMins += (fdate.xMins - useDisc);
                                    fdate.xMins -= useDisc;
                                }
                                else
                                {
                                    useDisc = 0;
                                    f24Pay += dayBasePrice;//使用未超過基本分,且折扣小於基本分時不能折
                                    wLastMins += fdate.xMins - useDisc;
                                    fdate.xMins -= useDisc;
                                }
                            }
                            tmpUseDisc -= useDisc;
                            f_wDisc += useDisc;
                        }
                    }
                    else if (fdate.DateType == hDateType)
                    {
                        if (useDisc == 0)
                        {
                            hLastMins += fdate.xMins;
                            f24Pay += (fdate.xMins - dayBaseMins) * priceHmin + dayBasePrice;
                        }
                        else
                        {
                            if (useDisc >= dayBaseMins)
                            {
                                hLastMins += (fdate.xMins - useDisc);
                                f24Pay += (fdate.xMins - useDisc) * priceHmin;
                                fdate.xMins -= useDisc;
                                fdate.useBaseMins = dayBaseMins;
                            }
                            else
                            {
                                if (f01_over6 > 0)
                                {
                                    useDisc = useDisc > f01_over6 ? f01_over6 : useDisc;
                                    //折扣小於基本分只能折扣超過基本分的部分
                                    f24Pay += dayBasePrice + ((fdate.xMins - dayBaseMins) - useDisc) * priceHmin;
                                    hLastMins += (fdate.xMins - useDisc);
                                    fdate.xMins -= useDisc;
                                }
                                else
                                {
                                    useDisc = 0;
                                    f24Pay += dayBasePrice;//使用未超過基本分,且折扣小於基本分時不能折
                                    wLastMins += fdate.xMins - useDisc;
                                    fdate.xMins -= useDisc;
                                }
                            }
                            tmpUseDisc -= useDisc;
                            f_hDisc += useDisc;
                        }
                    }
                }
                else if (FdateType == sLas)
                {
                    double useDisc = 0;
                    if (tmpUseDisc > 0)
                        useDisc = tmpUseDisc > fdate.xMins ? fdate.xMins : tmpUseDisc;

                    if (fdate.DateType == wDateType)
                    {
                        tmpUseDisc -= useDisc;
                        f_wDisc += useDisc;
                        f24Pay += (fdate.xMins - useDisc) * priceNmin;
                        wLastMins += (fdate.xMins - useDisc);
                        fdate.xMins -= useDisc;
                    }
                    else if (fdate.DateType == hDateType)
                    {
                        tmpUseDisc -= useDisc;
                        f_hDisc += useDisc;
                        f24Pay += (fdate.xMins - useDisc) * priceHmin;
                        hLastMins += (fdate.xMins - useDisc);
                        fdate.xMins -= useDisc;
                    }
                }

                UseDisc = f_wDisc + f_hDisc;    //使用點數
                if (UseDisc >= fDayMaxMins) //使用點數超過上限
                    f24Pay = 0;
                else if (f24Pay > dayMaxPrice)
                    f24Pay = dayMaxPrice;   //價格超過上限
            }

            return new Tuple<double, double, double, double, double>(f_wDisc, f_hDisc, f24Pay, wLastMins, hLastMins);
        }
        #endregion

        #region 取得區段內時間標記
        /// <summary>
        /// 取得區段內時間標記
        /// </summary>
        /// <param name="sd">起</param>
        /// <param name="ed">迄</param>
        /// <param name="lstHoliday">一班假日列表(必填HolidayDate)</param>
        /// <param name="mData">月租(必填MonthlyRentId,StartDate,EndDate)</param>
        /// <returns></returns>
        /// <mark>2020-12-09</mark>
        public List<DayPayMins> GetDateMark(DateTime sd, DateTime ed, List<Holiday> lstHoliday = null, List<MonthlyRentData> mData = null)
        {
            var re = new List<DayPayMins>();

            if (lstHoliday != null && lstHoliday.Count() > 0)
            {
                if (lstHoliday.Any(x => string.IsNullOrEmpty(x.HolidayDate) || string.IsNullOrWhiteSpace(x.HolidayDate)))
                    throw new Exception("HolidayDate必填");
            }

            if (mData != null && mData.Count() > 0)
            {
                if (mData.Any(x => x.StartDate == null || x.EndDate == null || x.EndDate < x.StartDate))
                    throw new Exception("起迄必填,迄日不可小於起日");

                if (mData.Any(x => x.MonthlyRentId <= 0))
                    throw new Exception("MonthlyRentId必填且不可為0或負數");

                if (mData.GroupBy(x => x.MonthlyRentId).Where(y => y.Count() > 1).Count() > 0)
                    throw new Exception("MonthlyRentId不可重複");

                //避免產月租日列表多算一天
                mData.ForEach(x =>
                {
                    var HHmmss = x.EndDate.ToString("HHmmss");
                    if (HHmmss == "000000")
                        x.EndDate = x.EndDate.AddSeconds(-1);
                });
            }

            if (lstHoliday != null && lstHoliday.Count() > 0)
            {
                if (mData != null && mData.Count() > 0)
                {
                    lstHoliday.ForEach(x => //月租假日
                    {
                        if (!re.Any(w => w.xDate == x.HolidayDate))
                        {
                            var mf = mData.Where(y => string.Compare(x.HolidayDate, y.StartDate.ToString("yyyyMMdd")) >= 0 && string.Compare(x.HolidayDate, y.EndDate.ToString("yyyyMMdd")) <= 0).FirstOrDefault();

                            if (mf != null)
                            {
                                DayPayMins d = new DayPayMins();
                                d.DateType = mf.MonthlyRentId.ToString() + "h";
                                d.xDate = x.HolidayDate;
                                re.Add(d);
                            }
                        }
                    });
                }

                var hDays = lstHoliday.Where(x => !re.Any(y => y.xDate == x.HolidayDate)).ToList();
                re.AddRange(FromHoliday(hDays));    //一般假日                
            }

            if (mData != null && mData.Count() > 0) //月租日期
                mData.ForEach(x => re = FillDateType(x.StartDate, x.EndDate, x.MonthlyRentId.ToString(), re));

            re = re.Where(x => string.Compare(x.xDate, sd.ToString("yyyyMMdd")) >= 0 && string.Compare(x.xDate, ed.ToString("yyyyMMdd")) <= 0).ToList();

            re = FillDate(sd, ed, re);
            re = re.OrderBy(x => x.xDate).ToList();

            return re;
        }
        #endregion

        #region 將指定時間類別給予GroupId及起訖
        /// <summary>
        /// 將指定時間類別給予GroupId及起訖
        /// </summary>
        /// <param name="dateTypes">時間類別</param>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-21 eason</mark>
        public List<DayPayMins> GetDateGroup(List<string> dateTypes, string grpFNamg, List<DayPayMins> sour)
        {
            string funNm = "GetDateGroup : ";
            if (dateTypes == null || dateTypes.Count() == 0)
                throw new Exception(funNm + "dateTypes 為必填");

            if (sour != null && sour.Count() > 0)
            {
                if (sour.Any(x => string.IsNullOrWhiteSpace(x.DateType) || string.IsNullOrWhiteSpace(x.xSTime)))
                    throw new Exception(funNm + "DateType 為必填");

                sour = sour.OrderBy(x => x.xSTime).ThenByDescending(y => y.haveNext).ToList();

                int grpId = 0;
                bool doAdd = false;
                for (int i = 0; i < sour.Count(); i++)
                {
                    var item = sour[i];
                    if (dateTypes.Any(a => a == item.DateType))
                    {
                        if (!doAdd)
                        {
                            doAdd = true;
                            grpId += 1;
                            item.isGrpStar = 1;
                        }
                        item.dayGroupId = grpFNamg + grpId.ToString();
                    }
                    else
                    {
                        if (doAdd)
                        {
                            doAdd = false;
                            if (i > 1)
                                sour[i - 1].isGrpEnd = 1;
                        }
                    }
                }
            }

            return sour;
        }
        #endregion

        #region 首日分鐘轉指定分鐘
        /// <summary>
        /// 首日分鐘轉指定分鐘,如機車首日200轉199
        /// </summary>
        /// <param name="sour"></param>
        /// <param name="oriMins">正常單日分鐘</param>
        /// <param name="fMins">首日分鐘</param>
        /// <returns></returns>
        /// <mark>2020-12-21 eason</mark>
        public List<DayPayMins> H24FDateSet(List<DayPayMins> sour, double oriMins, double fMins)
        {
            var re = new List<DayPayMins>();

            string funNm = "H24FDateCut : ";

            if (sour == null && sour.Count() == 0)
                throw new Exception(funNm + "sour為必填");
            if (oriMins <= 0 || fMins <= 0 || oriMins < fMins)
                throw new Exception(funNm + "oriMins或fMins錯誤");

            if (sour != null && sour.Count() > 0)
            {
                sour = sour.OrderBy(x => x.xSTime).ThenByDescending(y => y.haveNext).ToList();
                var fdate = sour.FirstOrDefault();
                fdate.isF24H = true;
                if (fdate.haveNext == 1)
                {
                    if (sour.Count() >= 2)
                        sour[1].isF24H = true;
                }

                var fDaysOntTppe = sour.Where(x => x.isF24H && x.isFull24H).OrderByDescending(y => y.haveNext).FirstOrDefault();
                if (fDaysOntTppe != null)
                {
                    fDaysOntTppe.xMins = fDaysOntTppe.xMins >= oriMins ? fMins : fDaysOntTppe.xMins;
                    re.Add(fDaysOntTppe);
                }

                var fDaysTwoTppe = sour.Where(x => x.isF24H && !x.isFull24H).ToList();
                if (fDaysTwoTppe != null && fDaysTwoTppe.Count() > 0)
                {
                    fDaysTwoTppe.ForEach(x =>
                    {
                        var fds = sour.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime).OrderByDescending(a => a.haveNext).ToList();
                        var fe = fds.LastOrDefault();
                        var allMins = fds.Select(z => z.xMins).Sum();
                        var diff = oriMins - fMins;
                        if (allMins >= oriMins)
                        {
                            fe.xMins = (fe.xMins - diff) > 0 ? (fe.xMins - diff) : fe.xMins;
                            re.Add(fe);
                        }
                    });
                }
            }

            if (re != null && re.Count() > 0)
            {
                sour.ForEach(x =>
                {
                    var d = re.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.isStart == x.isStart && y.isEnd == x.isEnd && y.haveNext == x.haveNext && y.DateType == x.DateType).FirstOrDefault();
                    if (d != null)
                        x = d;
                });
            }

            return sour;
        }
        #endregion

        #region 自動填入DateType
        /// <summary>
        /// 自動填入DateType,原本有DateType則不變更該筆
        /// </summary>
        /// <param name="sd">起</param>
        /// <param name="ed">迄</param>
        /// <param name="dateType">日期種類</param>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> FillDateType(DateTime sd, DateTime ed, string dateType, List<DayPayMins> sour = null)
        {
            List<DayPayMins> re = new List<DayPayMins>();
            if (sour != null && sour.Count() > 0)
                re = sour;

            if (sd != null && ed != null && ed >= sd)
            {
                DateTime sct = Convert.ToDateTime(sd.ToString("yyyy-MM-dd"));
                DateTime ect = Convert.ToDateTime(ed.ToString("yyyy-MM-dd"));

                while (sct <= ect)
                {
                    if (!re.Any(x => x.xDate == sct.ToString("yyyyMMdd")))
                    {
                        DayPayMins item = new DayPayMins()
                        {
                            DateType = dateType,
                            xDate = sct.ToString("yyyyMMdd")
                        };
                        re.Add(item);
                    }
                    sct = sct.AddDays(1);
                }
            }

            if (re != null && re.Count() > 0)
                re = re.OrderBy(x => x.xDate).ToList();

            return re;
        }
        #endregion

        #region 自動填平常日
        /// <summary>
        /// 自動填平常日
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> FillDate(DateTime sd, DateTime ed, List<DayPayMins> sour)
        {
            return FillDateType(sd, ed, eumDateType.wDay.ToString(), sour);
        }
        #endregion

        #region DayPayMins轉Holiday
        /// <summary>
        /// DayPayMins轉Holiday
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<Holiday> FromDayPayMins(List<DayPayMins> sour)
        {
            List<Holiday> re = new List<Holiday>();

            if (sour != null && sour.Count() > 0)
            {
                re = (from a in sour
                      select new Holiday
                      {
                          HolidayDate = a.xDate
                      }).ToList();
            }

            return re;
        }
        #endregion

        #region Holiday轉DayPayMins自動加DateType
        /// <summary>
        /// Holiday轉DayPayMins自動加DateType
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> FromHoliday(List<Holiday> sour)
        {
            List<DayPayMins> re = new List<DayPayMins>();

            if (sour != null && sour.Count() > 0)
            {
                re = (from a in sour
                      select new DayPayMins
                      {
                          DateType = eumDateType.hDay.ToString(),
                          xDate = a.HolidayDate
                      }).ToList();
            }
            return re;
        }
        #endregion

        #region 取得幾天幾小時幾分
        /// <summary>
        /// 取得幾天幾小時幾分
        /// </summary>
        /// <param name="sd">起</param>
        /// <param name="ed">迄</param>
        /// <param name="ProjType">專案類別</param>
        /// <returns></returns>
        /// <mark>2020-12-22 eason</mark>
        public Tuple<double, double, double> GetTimePart(DateTime sd, DateTime ed, int ProjType)
        {
            double days = 0;
            double hours = 0;
            double mins = 0;
            double dayBasMins = 0;
            double dayMaxMins = 0;

            sd = Convert.ToDateTime(sd.ToString("yyyy-MM-dd HH:mm"));
            ed = Convert.ToDateTime(ed.ToString("yyyy-MM-dd HH:mm"));

            if (ProjType == 4)
            {
                dayBasMins = 6;
                dayMaxMins = 600;   // 20220114 UPD BY YEH REASON:機車單日上限改為600分鐘
                var xre = GetMotoRangeMins(sd, ed, dayBasMins, dayMaxMins, new List<Holiday>());
                if (xre != null)
                {
                    var vre = GetTimePart(xre.Item1, dayMaxMins);
                    if (vre != null)
                    {
                        days += vre.Item1;
                        hours = vre.Item2;
                        mins = vre.Item3;
                    }
                }
            }
            else if (ProjType == 0 || ProjType == 3)
            {
                dayBasMins = 60;
                dayMaxMins = 600;
                var xre = GetCarRangeMins(sd, ed, dayBasMins, dayMaxMins, new List<Holiday>());
                if (xre != null)
                {
                    var vre = GetTimePart(xre.Item1, dayMaxMins);
                    if (vre != null)
                    {
                        days = vre.Item1;
                        hours = vre.Item2;
                        mins = vre.Item3;
                    }
                }
            }

            return new Tuple<double, double, double>(days, hours, mins);
        }

        /// <summary>
        /// 取得幾天幾小時幾分
        /// </summary>
        /// <param name="allMins"></param>
        /// <param name="dayMaxMins"></param>
        /// <returns></returns>
        public Tuple<double, double, double> GetTimePart(double allMins, double dayMaxMins)
        {
            double days = 0;
            double hours = 0;
            double mins = 0;
            if (allMins > 0)
            {
                days = Math.Floor(allMins / dayMaxMins);
                var h_mins = allMins % dayMaxMins;
                hours = Math.Floor(h_mins / 60);
                mins = h_mins % 60;
            }
            return new Tuple<double, double, double>(days, hours, mins);
        }
        #endregion

        public int DiscountLabelToPrice(DateTime SD, DateTime ED, int Price, int PriceH, double dayMaxHour, List<Holiday> lstHoliday, int GiveMinute, bool overTime = false, double baseMinutes = 60)
        {
            var re = 0;
            //優惠標籤折抵金額 要超過基本
            if (ED.Subtract(SD).TotalMinutes > baseMinutes)
            {

                var minsPro = new MinsProcess(GetCarPayMins);
                var dayPro = overTime ? new DayMinsProcess(CarOverTimeMinsToPayMins) : null;

                double workdayPriceHour = 0; //平日每小時價格
                double holidayPriceHour = 0; //假日每小時價格

                workdayPriceHour = Price / dayMaxHour;
                holidayPriceHour = PriceH / dayMaxHour;

                var result = GetRangeMins(SD, ED, baseMinutes, dayMaxHour * 60, lstHoliday, minsPro, dayPro);

                if (result != null)
                {
                    double tPrice = 0;

                    /*Todo 贈送分鐘數換算*/
                    double workdayGiveMinute = 0;
                    double holidayGiveMinute = 0;

                    //假日優惠分鐘數
                    holidayGiveMinute = (result.Item2 >= Convert.ToDouble(GiveMinute)) ? GiveMinute : result.Item2;
                    workdayGiveMinute = GiveMinute - holidayGiveMinute;

                    tPrice += Math.Floor((workdayGiveMinute / 60) * workdayPriceHour);
                    tPrice += Math.Floor((holidayGiveMinute / 60) * holidayPriceHour);

                    if (tPrice > 0)
                        re = Convert.ToInt32(tPrice);
                }
            }

            return re;
        }
    }

    #region eumDateType
    public enum eumDateType
    {
        /// <summary>
        /// 平日
        /// </summary>
        wDay,
        /// <summary>
        /// 假日
        /// </summary>
        hDay,
        /// <summary>
        /// 月租不分平假日
        /// </summary>
        m_Day,
        /// <summary>
        /// 月租平日
        /// </summary>
        m_wDay,
        /// <summary>
        /// 月租假日
        /// </summary>
        m_hDay,
    }
    #endregion
}