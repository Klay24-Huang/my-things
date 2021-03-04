using Domain.SP.Input.Bill;
using Domain.SP.Output.Bill;
using Domain.TB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 計費公用程式
    /// </summary>
    public class BillCommon
    {
        private const int oneDay = 1440; //24小時轉分鐘數
        private const int twoDay = 2880; //48小時轉分鐘數
        public float _scriptHolidayHour;
        public float _scriptWorkHour;
        public float _scriptTotalHour;
        public double _holidayHour;
        public double _normalHour;
        public double _scriptionRateWorkHourPrice;       //訂閱優惠費率時數（平日）
        public double _scriptionRateHolidayHourPrice;     //訂閱優惠費率時數（假日）
        public double _scriptionRateWorkHour;       //訂閱優惠費率時數（平日）
        public double _scriptionRateHolidayHour;     //訂閱優惠費率時數（假日）

        public delegate double MinsProcess(double mins);//剩餘分轉計費分(未滿60)
        public delegate void DayMinsProcess(ref double wMins, ref double hMins);//當日分特殊邏輯

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
            int dayNum = 0;
            double diffHours = 0;
            int diffMinutes = 0;
            int SDMinutes = 0;
            int EDMinutes = 0;
            int SDHours = 0;
            int EDHours = 0;
            double totalPay = 0;

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
        /// <summary>
        /// 由總分鐘數換算天數、時數、分鐘數
        /// </summary>
        /// <param name="TotalMinutes"></param>
        /// <param name="Days"></param>
        /// <param name="Hours"></param>
        /// <param name="Minutes"></param>
        public void CalMinuteToDayHourMin(int TotalMinutes, ref int Days, ref int Hours, ref int Minutes)
        {
            Days = Convert.ToInt32(Math.Floor(Convert.ToDouble(TotalMinutes / 600)));
            Hours = Convert.ToInt32(Math.Floor(Convert.ToDouble(((TotalMinutes % 600) / 60))));
            Minutes = TotalMinutes - (Days * 600) - (Hours * 60);

        }

        /// <summary>
        ///  計算以分計費金額(不分平假日)
        /// </summary>
        /// <param name="TotalMinutes"></param>
        /// <param name="BaseMinutes"></param>
        /// <param name="BasePrice"></param>
        /// <param name="Price"></param>
        /// <param name="PriceH"></param>
        /// <param name="MaxPrice"></param>
        /// <param name="TotalPrice"></param>
        public void CalFinalPriceByMinutes(int TotalMinutes, int BaseMinutes, int BasePrice, float Price, float PriceH, int MaxPrice, ref int TotalPrice)
        {
            if (TotalMinutes > 0 && TotalMinutes <= BaseMinutes)   //如果時間大於0且小於基本時數，則以基本費計算
            {
                TotalPrice = BasePrice;
            }
            else if (TotalMinutes > 0)
            {
                int days = 0, hours = 0, mins = 0;
                CalMinuteToDayHourMin(TotalMinutes - BaseMinutes, ref days, ref hours, ref mins); //取出天、時、分
                TotalPrice = Convert.ToInt32(Math.Floor((MaxPrice * days) + (((Price * 60 * hours) < MaxPrice) ? (Price * 60 * hours) : MaxPrice) + (mins * Price))) + BaseMinutes;
            }
            else
            {
                TotalPrice = 0; //時數是0
            }
        }

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
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetMilageSetting);
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
            finally
            {

            }
            return MilageBase;
        }
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
            //if (mins >= 15 && mins <= 30)
            //{
            //    hours += 0.5;
            //}
            //else if (mins > 30)
            //{
            //    hours += 1;
            //    mins = 0;
            //}
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
            int dayNum = 0;
            double diffHours = 0;
            int diffMinutes = 0;
            int SDMinutes = 0;
            int EDMinutes = 0;
            int SDHours = 0;
            int EDHours = 0;
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
        /// <returns></returns>
        public CarRentInfo CarRentInCompute(DateTime SD, DateTime ED, double priceN, double priceH, double daybaseMins, double dayMaxHour, List<Holiday> lstHoliday
            , List<MonthlyRentData> mOri
            , int Discount
            , double FreeMins = 0
            )
        {//note: CarRentInCompute2
            CarRentInfo re = new CarRentInfo();
            double dre = 0;
            double lastDisc = Convert.ToDouble(Discount);//剩餘折扣
            List<MonthlyRentData> mFinal = new List<MonthlyRentData>();//剩餘月租點數
            List<string> norDates = new List<string>()//一般平假日
            {
                eumDateType.wDay.ToString(),
                eumDateType.hDay.ToString()
            };

            if (Discount < 0)
                throw new Exception("折扣不可於0");

            if (Discount % 30 > 0)
                throw new Exception("折扣須為30的倍數");

            if (SD == null || ED == null || SD > ED)
                throw new Exception("SD,ED錯誤");
            
            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);
            var mins = ED.Subtract(SD).TotalMinutes;
            if (mins > 60)
                daybaseMins = 0;

            if (mOri != null && mOri.Count() > 0)
            {
                if (mOri.Any(x => x.WorkDayHours < 0 || x.WorkDayRateForCar < 0 ||
                   x.HolidayHours < 0 || x.HoildayRateForCar < 0 || x.MonthlyRentId <= 0
                   || x.Mode != 0
                ))
                    throw new Exception("mOri資料內容錯誤");

                if (mOri.GroupBy(x => x.MonthlyRentId).Where(y => y.Count() > 1).Count() > 0)
                    throw new Exception("MonthlyRentId不可重複");

                mOri = mOri.OrderByDescending(x => x.MonLvl).ToList();

                mFinal = objUti.Clone(mOri);

                //小時轉分
                mFinal.ForEach(x => { 
                    x.CarTotalHours = x.CarTotalHours * 60;
                    x.WorkDayHours = x.WorkDayHours * 60;
                    x.HolidayHours = x.HolidayHours * 60; 
                });
            }

            var allDay = GetDateMark(SD, ED, lstHoliday, mOri);//區間內時間註記
            var dayPayList = GetCarTypeMins(SD, ED, daybaseMins, dayMaxHour * 60, allDay);//全分類時間

            if(FreeMins > 0 && dayPayList != null && dayPayList.Count()>0)
                dayPayList = befMinsFree(FreeMins, dayPayList);

            var norList = dayPayList.Where(x => norDates.Any(y => y == x.DateType)).ToList();//一般時段
            var dpList = new List<DayPayMins>();//剩餘有分鐘數的

            re.RentInMins = Convert.ToInt32(Math.Floor(dayPayList.Select(x => x.xMins).Sum()));
            re.DiscRentInMins = Convert.ToInt32(norList.Select(x => x.xMins).Sum());

            //費率回存
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

            double wDisc = 0; //平日折扣
            double hDisc = 0; //假日則扣  
            double m_wDisc = 0; //平日折扣(所有月租)
            double m_hDisc = 0; //假日則扣(所有月租) 
            var mList = dayPayList.Where(x => !norDates.Any(y => y == x.DateType)).OrderBy(z => z.xDate).ToList();
            var mDiscs = new List<DayPayMins>();//有變動的日期 

            //月租內點數先折
            if (mFinal != null && mFinal.Count() > 0)
            {
                foreach (var m in mFinal)
                {
                    var carAllDisc = Convert.ToDouble(m.CarTotalHours);                    
                    var wdisc = Convert.ToDouble(m.WorkDayHours - m.WorkDayHours % 30);//平日可折
                    var hdisc = Convert.ToDouble(m.HolidayHours - m.HolidayHours % 30);//假日可折

                    var allDays = mList.Where(x => x.DateType == m.MonthlyRentId.ToString() || x.DateType == (m.MonthlyRentId.ToString() + "h")).OrderBy(y => y.xDate).ToList();
                    var wDays = mList.Where(x => x.DateType == m.MonthlyRentId.ToString()).OrderBy(y => y.xDate).ToList();
                    var hDays = mList.Where(x => x.DateType == (m.MonthlyRentId.ToString() + "h")).OrderBy(y => y.xDate).ToList();

                    if(carAllDisc > 0)
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

                        m.CarTotalHours -= (Convert.ToSingle(carAllDisc));//使用點數
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

                        m.WorkDayHours -= Convert.ToSingle(wdisc);//使用點數
                        m.HolidayHours -= Convert.ToSingle(hdisc);//使用點數
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
                    {
                        m_hDisc += useDisc;
                        var mm = mFinal.Where(w => w.MonthlyRentId.ToString() == x.DateType.Replace("h", string.Empty).Trim()).FirstOrDefault();
                        mm.HolidayHours -= Convert.ToSingle(useDisc);
                    }
                    else
                    {
                        m_wDisc += useDisc;
                        var mm = mFinal.Where(w => w.MonthlyRentId.ToString() == x.DateType).FirstOrDefault();
                        mm.WorkDayHours -= Convert.ToSingle(useDisc);
                    }
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
            re.useMonthDisc = m_wDisc + m_hDisc;
            re.lastMonthDisc = mOri.Select(x => x.WorkDayHours * 60).Sum() +
                mOri.Select(x => x.HolidayHours * 60).Sum() - (m_wDisc + m_hDisc);

            if (mFinal != null && mFinal.Count() > 0)//回傳monthData
            {
                mFinal.ForEach(x =>
                {
                    x.CarTotalHours = (x.CarTotalHours / 60);
                    x.HolidayHours = (x.HolidayHours / 60);//分轉回小時
                    x.WorkDayHours = (x.WorkDayHours / 60);//分轉回小時                    
                });
                re.mFinal = mFinal;
            }

            dre = dre > 0 ? dre : 0;

            re.useMonthDiscW = m_wDisc;
            re.useMonthDiscH = m_hDisc;

            re.RentInPay = Convert.ToInt32(Math.Round(dre, 0, MidpointRounding.AwayFromZero));

            return re;
        }

        /// <summary>
        /// 機車月租計算,區分平假日,不分平假日
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="priceNmin"></param>
        /// <param name="priceHmin"></param>
        /// <param name="dayBaseMins"></param>
        /// <param name="dayMaxMins"></param>
        /// <param name="lstHoliday"></param>
        /// <param name="mOri"></param>
        /// <param name="Discount"></param>
        /// <param name="fDayMaxMins"></param>
        /// <param name="fDayMaxPrice"></param>
        /// <param name="dayBasePrice"></param>
        /// <returns></returns>
        public CarRentInfo MotoRentMonthComp(DateTime SD, DateTime ED, double priceNmin, double priceHmin, int dayBaseMins, double dayMaxMins
             , List<Holiday> lstHoliday = null
             , List<MonthlyRentData> mOri = null
             , int Discount = 0
             , int fDayMaxMins = 0
             , double fDayMaxPrice = 0
             , double dayBasePrice = 10
             , double FreeMins = 0
            )
        {//note: MotoRentMonthComp2
            CarRentInfo re = new CarRentInfo();
            double dre = 0;
            List<MonthlyRentData> mFinal = new List<MonthlyRentData>();//剩餘月租點數
            List<string> norDates = new List<string>()//一般平假日
            {
                eumDateType.wDay.ToString(),
                eumDateType.hDay.ToString()
            };

            if (SD == null || ED == null || SD > ED)
                throw new Exception("SD,ED資料錯誤");

            if (Discount < 0)
                throw new Exception("折扣不可為負數");

            double nowDisc = Convert.ToDouble(Discount);

            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);
            double mins = ED.Subtract(SD).TotalMinutes;//真實分鐘

            if (mOri != null && mOri.Count() > 0)
            {
                if (mOri.Any(x => x.MotoTotalHours < 0 || x.WorkDayRateForMoto < 0 ||
                   x.HoildayRateForMoto < 0 || x.MonthlyRentId <= 0
                   || x.Mode != 1
                ))
                    throw new Exception("mOri資料內容錯誤");

                if (mOri.GroupBy(x => x.MonthlyRentId).Where(y => y.Count() > 1).Count() > 0)
                    throw new Exception("MonthlyRentId不可重複");

                mOri = mOri.OrderByDescending(x => x.MonLvl).ToList();

                mFinal = objUti.Clone(mOri);
            }

            var allDay = GetDateMark(SD, ED, lstHoliday, mOri);//區間內時間註記
            var dayPayList = GetMotoTypeMins(SD, ED, dayBaseMins, dayMaxMins, allDay);//全分類時間 
            dayPayList = H24FDateSet(dayPayList, dayMaxMins, fDayMaxMins);//縮減首日及mark首日200=>199

            if (FreeMins > 0 && dayPayList != null && dayPayList.Count() > 0)
                dayPayList = befMinsFree(FreeMins, dayPayList);

            var norList = dayPayList.Where(x => norDates.Any(y => y == x.DateType)).ToList();//一般時段
            var dpList = new List<DayPayMins>();//剩餘有分鐘數的

            if (dayPayList != null && dayPayList.Count() > 0)
            {
                re.RentInMins = Convert.ToInt32(dayPayList.Select(x => x.xMins).Sum());
                //費率回存
                dayPayList.ForEach(x =>
                {
                    if (x.DateType == eumDateType.wDay.ToString())
                        x.xRate = priceNmin;
                    else if (x.DateType == eumDateType.hDay.ToString())
                        x.xRate = priceHmin;
                    else if (x.DateType.Contains("h"))
                    {
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType.Replace("h", string.Empty).Trim()).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.HoildayRateForMoto;
                    }
                    else
                    {
                        var m = mFinal.Where(y => y.MonthlyRentId.ToString() == x.DateType).FirstOrDefault();
                        if (m != null)
                            x.xRate = m.WorkDayRateForMoto;
                    }
                });
                //取GroupId
                dayPayList = GetDateGroup(norDates, "nor_", dayPayList);
            }

            if (norList != null && norList.Count() > 0)
                re.DiscRentInMins = Convert.ToInt32(norList.Select(x => x.xMins).Sum());

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
                    double m_discW = Convert.ToDouble(m.MotoWorkDayMins);
                    double m_discH = Convert.ToDouble(m.MotoHolidayMins);
                    var m_wType = m.MonthlyRentId.ToString();
                    var m_hType = m_wType + "h";

                    //不分平假日
                    if (m_disc > 0)
                    {
                        var m_list = mList.Where(x => x.DateType == m_wType || x.DateType == m_hType)
                            .OrderBy(y => y.xSTime).ThenByDescending(w => w.haveNext).ToList();
                        if (m_list != null && m_list.Count() > 0)
                        {
                            if (m_list != null && mList.Count() > 0)
                                m_list.ForEach(x => x.dayGroupId = "mon_" + m.MonthlyRentId.ToString());

                            var mre = MotoRentDiscComp(m.WorkDayRateForMoto, m.HoildayRateForMoto, dayBaseMins, dayBasePrice, ref m_list, m_disc, m_wType, m_hType, fDayMaxMins, fDayMaxPrice);
                            if (mre != null)
                            {
                                dre += mre.Item3;
                                m_wDisc += mre.Item1;
                                m_hDisc += mre.Item2;
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
                    //分平假日
                    else
                    {
                        var m_wliset = mList.Where(x => x.DateType == m_wType)
                            .OrderBy(y => y.xSTime).ThenByDescending(w => w.haveNext).ToList();

                        var m_hliset = mList.Where(x => x.DateType == m_hType)
                            .OrderBy(y => y.xSTime).ThenByDescending(w => w.haveNext).ToList();

                        if (m_wliset != null && m_wliset.Count() > 0)
                        {
                            var mre = MotoRentDiscComp(m.WorkDayRateForMoto, m.HoildayRateForMoto, dayBaseMins, dayBasePrice, ref m_wliset, m_discW, m_wType, m_hType, fDayMaxMins, fDayMaxPrice);
                            if (mre != null)
                            {
                                dre += mre.Item3;
                                m_wDisc += mre.Item1;
                                m_hDisc += mre.Item2;
                                m.MotoWorkDayMins -= Convert.ToSingle(mre.Item1 + mre.Item2);
                            }

                            //還原變動
                            dayPayList.ForEach(x =>
                            {
                                var item = m_wliset.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
                                if (item != null)
                                    x = item;
                            });
                        }

                        if (m_hliset != null && m_hliset.Count() > 0)
                        {
                            var mre = MotoRentDiscComp(m.WorkDayRateForMoto, m.HoildayRateForMoto, dayBaseMins, dayBasePrice, ref m_hliset, m_discH, m_wType, m_hType, fDayMaxMins, fDayMaxPrice);
                            if (mre != null)
                            {
                                dre += mre.Item3;
                                m_wDisc += mre.Item1;
                                m_hDisc += mre.Item2;
                                m.MotoHolidayMins -= Convert.ToSingle(mre.Item1 + mre.Item2);
                            }

                            //還原變動
                            dayPayList.ForEach(x =>
                            {
                                var item = m_hliset.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime && y.haveNext == x.haveNext).FirstOrDefault();
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
            if (norList != null && norList.Count() > 0)
            {
                List<string> gIDs = norList.GroupBy(x => x.dayGroupId).Select(y => y.FirstOrDefault().dayGroupId).OrderBy(z => z).ToList();
                if (gIDs != null && gIDs.Count() > 0)
                {
                    foreach (var gID in gIDs)
                    {
                        var gList = norList.Where(x => x.dayGroupId == gID).OrderBy(y => y.xSTime).ThenByDescending(z => z.haveNext).ToList();
                        var gre = MotoRentDiscComp(priceNmin, priceHmin, dayBaseMins, dayBasePrice, ref gList, 0, eumDateType.wDay.ToString(), eumDateType.hDay.ToString(), fDayMaxMins, fDayMaxPrice);
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
                            if (useDisc >= dayBaseMins)
                            {
                                if (dayBasePrice > (dayBaseMins * x.xRate))//首6分鐘少1元":10-(1.5*6) = 1
                                    dre -= (dayBasePrice - (dayBaseMins * x.xRate));
                                else
                                    dre += (dayBaseMins * x.xRate) - dayBasePrice;
                                x.useBaseMins += dayBaseMins;
                            }
                            else
                            {
                                var f01_over6 = x.xMins - dayBaseMins;
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

            re.useDisc = Convert.ToInt32(wDisc + hDisc);//使用一般折扣點數
            re.useMonthDisc = m_wDisc + m_hDisc;//使用月租折扣點數

            if (mOri != null && mOri.Count() > 0)//剩餘月租點數
                re.lastMonthDisc = mOri.Select(x => x.MotoWorkDayMins + x.MotoHolidayMins).Sum() - (m_wDisc + m_hDisc);

            if (mFinal != null && mFinal.Count() > 0)//回傳monthData
            {
                mFinal.ForEach(x =>
                {
                    var mo = mOri.Where(y => y.MonthlyRentId == x.MonthlyRentId).FirstOrDefault();
                    if (mo != null)
                    {
                        x.MotoWorkDayMins = (mo.MotoWorkDayMins - x.MotoWorkDayMins);//使用的點數
                        x.MotoHolidayMins = (mo.MotoHolidayMins - x.MotoHolidayMins);
                    }
                });

                mFinal = mFinal.Where(x => x.MotoWorkDayMins > 0 || x.MotoHolidayMins > 0).ToList();//月租點數有使用才回傳
                re.mFinal = mFinal;
            }

            dre = dre > 0 ? dre : 0;

            re.useMonthDiscW = m_wDisc;
            re.useMonthDiscH = m_hDisc;

            re.RentInPay = Convert.ToInt32(Math.Round(dre, 0, MidpointRounding.AwayFromZero));

            return re;
        }

        /// <summary>
        /// 機車租金計算(月租平日折扣,月租假日折扣,折扣後金額,月租平日剩餘分,月租假日剩餘分,月租折扣後剩餘分鐘,使用一般折扣點數)
        /// </summary>
        /// <param name="priceNmin">平日價格(分)</param>
        /// <param name="priceHmin">假日價格(分)</param>
        /// <param name="dayBaseMins">基本分鐘數</param>
        /// <param name="dayBasePrice">基消</param>
        /// <param name="Discount">折扣</param>
        /// <param name="norList">日期及使用分列表</param>
        /// <param name="Discount">折扣點數</param>
        /// <param name="wDateType">平日日期註記</param>
        /// <param name="hDateType">假日日期註記</param>
        /// <param name="fDayMaxMins">首日最大計費分鐘</param>
        /// <param name="fDayMaxPrice">首日最大計費金額</param>
        /// <mark>2020-12-21 eason</mark>
        public Tuple<double, double, double, double, double, double, double> MotoRentDiscComp(
            double priceNmin, double priceHmin, double dayBaseMins, double dayBasePrice,
            ref List<DayPayMins> norList, double Discount,
            string wDateType, string hDateType,            
            double fDayMaxMins, 
            double fDayMaxPrice
            )
        {//note: MotoRentDiscComp
            #region 變數宣告
            //double fDayMaxMins = 199;//首24H最大計費分鐘
            //double dayMaxPrice = 300;//每24H最大租金
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
                    if (x.DateType == eumDateType.wDay.ToString())
                        x.xRate = priceNmin;
                    else if (x.DateType == eumDateType.hDay.ToString())
                        x.xRate = priceHmin;
                });

                norMins = norList.Select(x => x.xMins).Sum();//一般時段總租用分鐘
                nowDisc = nowDisc > norMins ? norMins : nowDisc;//自動縮減              

                if(fList != null && fList.Count() > 0)
                {
                    var fd = MotoF24HDiscCompute(priceNmin, priceHmin, dayBaseMins, dayBasePrice, fDayMaxPrice, nowDisc, ref fList, wDateType, hDateType, fDayMaxMins);
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

        /// <summary>
        /// 機車租金試算
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="PriceMin">每分鐘多少</param>
        /// <param name="baseMinutes">基本分鐘數</param>
        /// <param name="dayMaxPrice">單日計費上限</param>
        /// <param name="disc">折扣點數</param>
        /// <returns></returns>
        /// <mark>2020-11-19 eason</mark>
        public int MotoRentCompute(DateTime SD, DateTime ED, double PriceMin, int baseMinutes, double dayMaxPrice, int disc = 0)
        {
            int re = 0;

            if (disc < 0)
                throw new Exception("折扣不可為負數");

            if (disc > 0 && disc < 6)
                throw new Exception("折扣不可低於6分鐘");

            double dayMaxMins = dayMaxPrice / PriceMin;//單日上限分鐘 

            SD = SD.AddSeconds(SD.Second * -1);
            ED = ED.AddSeconds(ED.Second * -1);

            double mins = ED.Subtract(SD).TotalMinutes;
            double fpay = 0;

            if (mins < 24 * 60)
            {
                //先從基消判斷
                if (mins <= 6)
                    fpay = 10;
                else if (mins > dayMaxMins)
                    fpay = dayMaxPrice;
                else
                    fpay = (mins - baseMinutes) * PriceMin + 10;

                if (disc >= 199)
                    return 0;
            }
            else
            {
                var result = GetRangeMins(SD, ED, baseMinutes, dayMaxMins, new List<Holiday>());
                if (result != null)
                {
                    double payMins = result.Item1;
                    fpay = payMins * PriceMin;
                }
            }
            //折抵通則
            //折抵優先折抵基本分鐘，除非剩餘折抵時數小於基本分鐘，則只能折抵非基本分鐘數
            if (disc > 0)
            {
                if (disc < 199)
                    fpay = fpay - 10 - (disc - 6) * PriceMin;
                else
                    fpay = (fpay - 300) - (disc - 199) * PriceMin;
            }

            fpay = fpay >= 0 ? fpay : 0;
            re = Convert.ToInt32(Math.Round(fpay, 0, MidpointRounding.AwayFromZero));
            return re;
        }

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

        /// <summary>
        /// 取得真實折扣分鐘,平日折扣分鐘,假日折扣分鐘
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘</param>
        /// <param name="dayMaxMins">單日計費分鐘上限</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <param name="Discount">預備折扣點數</param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public Tuple<double, double, double> CarDiscToPara(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday, int Discount)
        {//note: CarDiscToPara
            double payDisc = 0; //真實使用折扣
            double wDisc = 0; //平日折扣
            double hDisc = 0; //假日則扣           

            double disc = Convert.ToDouble(Discount);

            if (disc < 0)
                throw new Exception("折扣不可於0");

            if (disc % 30 > 0)
                throw new Exception("折扣須為30的倍數");

            var dayPayList = GetCarRangeDayFlow(SD, ED, baseMinutes, dayMaxMins, lstHoliday);

            if (dayPayList != null && dayPayList.Count() > 0)
            {
                double mins = dayPayList.Select(x => x.xMins).Sum();

                if (disc > mins)
                    disc = mins; //自動縮減

                dayPayList.ForEach(x =>
                {
                    if (disc > 0)
                    {
                        if (disc >= x.xMins)
                        {
                            wDisc += x.isMarkDay ? 0 : x.xMins;
                            hDisc += x.isMarkDay ? x.xMins : 0;
                            payDisc += x.xMins;
                            disc -= x.xMins;
                        }
                        else
                        {
                            wDisc += x.isMarkDay ? 0 : disc;
                            hDisc += x.isMarkDay ? disc : 0;
                            payDisc += disc;
                            disc = 0;
                        }
                    }
                });
            }

            return new Tuple<double, double, double>(payDisc, wDisc, hDisc);
        }

        /// <summary>
        /// 未逾時計費分鐘by時間順序
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘數</param>
        /// <param name="dayMaxMins">日最大計費分鐘數</param>
        /// <param name="lstHoliday">假日列表</param>
        /// <returns></returns>
        /// <mark>2020-12-03 eason</mark>
        public List<DayPayMins> GetCarRangeDayFlow(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            MinsProcess minsPro = new MinsProcess(GetCarPayMins);
            var re = GetRangeDayFlow(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);

            if (re != null && re.Count() > 0)
                re = re.OrderBy(x => x.xDate).ToList();

            return re;
        }

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
        {//note:GetCarTypeMins
            var minsPro = new MinsProcess(GetCarPayMins);
            return GetTypeMins(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro);
        }

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
        {//note:GetMotoTypeMins
            var minsPro = new MinsProcess(GetMotoPayMins);
            return GetTypeMins(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro);
        }

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
        {//note: GetTypeMins
            List<DayPayMins> re = new List<DayPayMins>();

            if (markDays != null && markDays.Count() > 0)
                markDays = FillDate(SD, ED, markDays);

            var res = GetTypeDayFlow(SD, ED, baseMinutes, dayMaxMins, markDays, minsPro, dayPro);

            if (res != null && res.Count() > 0)
                re = res.OrderBy(x => x.xDate).ToList();

            return re;
        }

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

        /// <summary>
        /// 依時間軸取得則扣點數(平日點,假日點)
        /// </summary>
        /// <param name="SD">起</param>
        /// <param name="ED">迄</param>
        /// <param name="baseMinutes">基本分鐘</param>
        /// <param name="dayMaxMins">單日計費分鐘上限</param>
        /// <param name="lstHoliday">假日</param>
        /// <returns>平日可折點數,假日點可折點數</returns>
        public List<DayPayMins> GetRangeDayFlow(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday, MinsProcess minsPro = null, DayMinsProcess dayPro = null)
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

            bool sd_isHoliday = lstHoliday.Any(x => x.HolidayDate == str_sd);
            bool ed_isHoliday = lstHoliday.Any(x => x.HolidayDate == str_ed);

            double mins = ED.Subtract(SD).TotalMinutes;

            if (SD.Date == ED.Date || SD.AddDays(1) >= ED)
            {
                var xre = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
                if (xre != null && xre.Count > 0)
                    re.AddRange(xre);
            }
            else
            {
                while (SD < ED)
                {
                    var sd24 = SD.AddHours(24);
                    if (ED > sd24)
                    {
                        var re24 = GetH24DayPayList(SD, sd24, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
                        if (re24 != null && re24.Count() > 0)
                            re.AddRange(re24);
                    }
                    else
                    {
                        var reLast = GetH24DayPayList(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
                        if (reLast != null && reLast.Count() > 0)
                            re.AddRange(reLast);
                    }
                    SD = sd24;
                }
            }

            return re;
        }

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
        {//note: GetH24DayPayList
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

        //汽車未超時
        public Tuple<double, double> GetCarRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetCarPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);
        }

        //機車未超時
        public Tuple<double, double> GetMotoRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetMotoPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);
        }

        //汽車超時時間-計算用
        //2020-12-03 eason
        public Tuple<double, double> GetCarOutComputeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {//note: GetCarOutMins
            var minsPro = new MinsProcess(GetCarPayMins);
            var dayPro = new DayMinsProcess(CarOverTimeMinsToPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro, dayPro);
        }
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

        private void insSubScription(DateTime Date, DateTime StartDate, DateTime EndDate, bool isHoliday, double tmpHours, Int64 SubScriptionID, ref List<MonthlyRentData> UseMonthlyRentDatas)
        {
            if (isHoliday)
            {
                if (UseMonthlyRentDatas == null)
                {
                    UseMonthlyRentDatas = new List<MonthlyRentData>();
                    UseMonthlyRentDatas.Add(new MonthlyRentData()
                    {
                        MonthlyRentId = SubScriptionID,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        MotoTotalHours = 0,
                        HolidayHours = Convert.ToSingle(tmpHours),
                        WorkDayHours = 0
                    });
                    _scriptHolidayHour += Convert.ToSingle(tmpHours);
                }
                else
                {

                    UseMonthlyRentDatas.Add(new MonthlyRentData()
                    {
                        MonthlyRentId = SubScriptionID,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        MotoTotalHours = 0,
                        HolidayHours = Convert.ToSingle(tmpHours),
                        WorkDayHours = 0
                    });
                    _scriptHolidayHour += Convert.ToSingle(tmpHours);
                }
            }
            else
            {
                if (UseMonthlyRentDatas == null)
                {
                    UseMonthlyRentDatas = new List<MonthlyRentData>();
                    UseMonthlyRentDatas.Add(new MonthlyRentData()
                    {
                        MonthlyRentId = SubScriptionID,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        MotoTotalHours = 0,
                        HolidayHours = 0,
                        WorkDayHours = Convert.ToSingle(tmpHours)
                    });
                    _scriptWorkHour += Convert.ToSingle(tmpHours);
                }
                else
                {

                    UseMonthlyRentDatas.Add(new MonthlyRentData()
                    {
                        MonthlyRentId = SubScriptionID,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        MotoTotalHours = 0,
                        HolidayHours = 0,
                        WorkDayHours = Convert.ToSingle(tmpHours)
                    });
                    _scriptWorkHour += Convert.ToSingle(tmpHours);
                }
            }

        }
        public int CalBillBySubScription(DateTime SD, DateTime ED, List<Holiday> holidayList, int Price, int PriceH, ref string errCode, ref List<MonthlyRentData> lstSubScript, ref List<MonthlyRentData> ListUseMonthly)
        {
            double normalHour = 0.0;
            double holidayHour = 0.0;
            double totalHour = 0.0;


            this.HolidayRantCarBySubScription(holidayList, SD, ED, ref normalHour, ref holidayHour, ref totalHour, ref lstSubScript, ref ListUseMonthly);
            if (ListUseMonthly != null)
            {
                if (ListUseMonthly.Count > 0)
                {
                    _scriptionRateHolidayHourPrice = lstSubScript[0].HoildayRateForCar;
                    _scriptionRateWorkHourPrice = lstSubScript[0].WorkDayRateForCar;
                }
            }
            int holidayAllBill = (int)(holidayHour * (double)PriceH);
            int normalAllBill = (int)(normalHour * (double)Price);
            int scriptRateH = (int)(_scriptionRateHolidayHour * _scriptionRateHolidayHourPrice);
            int scriptRateW = (int)(_scriptionRateWorkHour * _scriptionRateWorkHourPrice);
            int bill = normalAllBill + holidayAllBill + scriptRateH + scriptRateW;
            return bill;
        }
        public void HolidayRantCarBySubScription(List<Holiday> holidayList, DateTime rantStart, DateTime rantEnd, ref double normalHour, ref double holidayHour, ref double totalHour, ref List<MonthlyRentData> lstSubScript, ref List<MonthlyRentData> ListUseMonthly)
        {
            //總時數
            TimeSpan totalSub = rantEnd - rantStart;

            normalHour = 0.0;
            holidayHour = 0.0;
            totalHour = 0.0;

            //抓出總小時數
            totalHour = GetHour(totalSub);
            int len = lstSubScript.Count;
            //先處理掉未滿一小時的租約，因為第二個小時15分鐘內不用錢，所以用小於75分鐘
            if (totalSub.TotalMinutes < 75)
            {
                //如果總時間小於15分鐘，就不用去看是否跨日，直接用取車時間算一小時的錢
                if (totalSub.TotalMinutes < 15)
                {
                    // int len = lstSubScript.Count;
                    bool hasInDate = false;
                    for (int i = 0; i < len; i++)
                    {
                        if (IsInDate(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                        {
                            //檢查是否為假日
                            if (IsInHoliday(holidayList, rantStart))
                            {
                                if (lstSubScript[i].HolidayHours >= 1)
                                {
                                    holidayHour = 0;
                                    lstSubScript[i].HolidayHours -= 1;
                                    insSubScription(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }


                            }
                            else
                            {
                                if (lstSubScript[i].WorkDayHours >= 1)
                                {
                                    normalHour = 0;
                                    lstSubScript[i].WorkDayHours -= 1;
                                    insSubScription(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }
                                // normalHour = 1;
                            }

                        }
                    }
                    if (hasInDate == false)
                    {

                        bool hasRate = checkInRateDate(rantStart, lstSubScript);
                        if (IsInHoliday(holidayList, rantStart))
                        {
                            holidayHour = 1;
                        }
                        else
                        {
                            if (hasRate)
                            {
                                _scriptionRateWorkHour = 1;
                            }
                            else
                            {
                                normalHour = 1;
                            }

                        }
                    }

                    return;
                }

                //抓出離下個明天有多久
                DateTime nextAccessDate = rantStart.AddMinutes(14);

                //如果加了15分鐘還是一樣的日期，直接用取車時間算一小時的錢
                if (rantStart.Date == nextAccessDate.Date)
                {
                    //檢查是否為假日

                    bool hasInDate = false;
                    for (int i = 0; i < len; i++)
                    {
                        if (IsInDate(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                        {
                            //檢查是否為假日
                            if (IsInHoliday(holidayList, rantStart))
                            {
                                if (lstSubScript[i].HolidayHours >= 1)
                                {
                                    holidayHour = 0;
                                    lstSubScript[i].HolidayHours -= 1;
                                    insSubScription(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }


                            }
                            else
                            {
                                if (lstSubScript[i].WorkDayHours >= 1)
                                {
                                    normalHour = 0;
                                    lstSubScript[i].WorkDayHours -= 1;
                                    insSubScription(rantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }
                                // normalHour = 1;
                            }

                        }
                    }
                    if (hasInDate == false)
                    {
                        bool hasRate = checkInRateDate(rantStart, lstSubScript);
                        if (IsInHoliday(holidayList, rantStart))
                        {
                            holidayHour = 1;
                        }
                        else
                        {
                            if (hasRate)
                            {
                                _scriptionRateWorkHour = 1;
                            }
                            else
                            {
                                normalHour = 1;
                            }

                        }
                    }

                    return;
                }
                //如果日期不一樣，就用下一個日期算錢
                else
                {
                    bool hasInDate = false;
                    for (int i = 0; i < len; i++)
                    {
                        if (IsInDate(nextAccessDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                        {
                            //檢查是否為假日
                            if (IsInHoliday(holidayList, nextAccessDate))
                            {
                                if (lstSubScript[i].HolidayHours >= 1)
                                {
                                    holidayHour = 0;
                                    lstSubScript[i].HolidayHours -= 1;
                                    insSubScription(nextAccessDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }


                            }
                            else
                            {
                                if (lstSubScript[i].WorkDayHours >= 1)
                                {
                                    normalHour = 0;
                                    lstSubScript[i].WorkDayHours -= 1;
                                    insSubScription(nextAccessDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, 1, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                    hasInDate = true;
                                }
                                // normalHour = 1;
                            }

                        }
                    }
                    if (hasInDate == false)
                    {

                        bool hasRate = checkInRateDate(nextAccessDate, lstSubScript);
                        if (IsInHoliday(holidayList, nextAccessDate))
                        {
                            holidayHour = 1;
                        }
                        else
                        {
                            if (hasRate)
                            {
                                _scriptionRateWorkHour = 1;
                            }
                            else
                            {
                                normalHour = 1;
                            }

                        }
                    }
                    return;
                }

            }

            //開始處理大於一小時的租約
            DateTime tempRantStart = rantStart;
            //用來處理第一天15分鐘問題
            bool isFirstDay = true;
            while (true)
            {
                //先抓出下一個日期
                DateTime nextDate = new DateTime(tempRantStart.Year, tempRantStart.Month, tempRantStart.Day);
                nextDate = nextDate.AddDays(1);

                //先抓出開始時間加一天
                DateTime nextRantStart = tempRantStart.AddDays(1);

                //還車時間小於等於下一個日期
                if (rantEnd <= nextDate)
                {
                    bool hasInDate = false;
                    //結算
                    double tempHour = GetHour((rantEnd - tempRantStart), true);
                    if (tempHour > 10)
                    {
                        tempHour = 10;
                    }

                    for (int i = 0; i < len; i++)
                    {
                        if (IsInDate(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                        {
                            if (IsInHoliday(holidayList, tempRantStart))
                            {
                                if (lstSubScript[i].HolidayHours > 0)
                                {
                                    if (lstSubScript[i].HolidayHours >= tempHour)
                                    {
                                        holidayHour += 0;
                                        lstSubScript[i].HolidayHours -= Convert.ToSingle(tempHour);
                                        insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, tempHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                        hasInDate = true;
                                    }
                                    else
                                    {
                                        holidayHour += tempHour - lstSubScript[i].HolidayHours;
                                        insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, lstSubScript[i].HolidayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                        lstSubScript[i].HolidayHours = 0;
                                        hasInDate = true;
                                    }
                                }

                            }
                            else
                            {
                                if (lstSubScript[i].WorkDayHours > 0)
                                {
                                    if (lstSubScript[i].WorkDayHours >= tempHour)
                                    {
                                        normalHour += 0;
                                        lstSubScript[i].WorkDayHours -= Convert.ToSingle(tempHour);
                                        insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, tempHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                        hasInDate = true;
                                    }
                                    else
                                    {

                                        if (checkInRateDate(tempRantStart, lstSubScript))
                                        {
                                            _scriptionRateWorkHour += tempHour - lstSubScript[i].WorkDayHours;
                                        }
                                        else
                                        {
                                            normalHour += tempHour - lstSubScript[i].WorkDayHours;
                                        }

                                        insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, lstSubScript[i].WorkDayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                        lstSubScript[i].WorkDayHours = 0;
                                        hasInDate = true;
                                    }
                                }

                            }
                            if (hasInDate)
                            {
                                break;
                            }
                        }
                    }

                    if (hasInDate == false)
                    {

                        bool hasRate = checkInRateDate(tempRantStart, lstSubScript);
                        if (IsInHoliday(holidayList, tempRantStart))
                        {

                            holidayHour += tempHour;

                        }
                        else
                        {
                            if (hasRate)
                            {
                                _scriptionRateWorkHour += tempHour;
                            }
                            else
                            {
                                normalHour += tempHour;
                            }

                        }
                    }




                    break;
                }
                else
                {
                    //先查看S~2是否超過十小時
                    double tempPreHour = GetHour((nextDate - tempRantStart), true);
                    double tempPreOriginHour = GetHour((nextDate - tempRantStart), true);
                    bool hasInDate = false;

                    if (tempPreHour > 10)
                    {
                        //如果大於十就直接給十
                        tempPreHour = 10;

                        for (int i = 0; i < len; i++)
                        {
                            if (IsInDate(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                            {
                                if (IsInHoliday(holidayList, tempRantStart))
                                {
                                    if (lstSubScript[i].HolidayHours > 0)
                                    {
                                        if (lstSubScript[i].HolidayHours >= tempPreHour)
                                        {
                                            holidayHour += 0;
                                            lstSubScript[i].HolidayHours -= Convert.ToSingle(tempPreHour);
                                            insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                            hasInDate = true;
                                        }
                                        else
                                        {

                                            holidayHour += tempPreHour - lstSubScript[i].HolidayHours;
                                            insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, lstSubScript[i].HolidayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                            lstSubScript[i].HolidayHours = 0;
                                            hasInDate = true;
                                        }
                                    }

                                }
                                else
                                {
                                    if (lstSubScript[i].WorkDayHours > 0)
                                    {
                                        if (lstSubScript[i].WorkDayHours >= tempPreHour)
                                        {
                                            normalHour += 0;
                                            lstSubScript[i].WorkDayHours -= Convert.ToSingle(tempPreHour);
                                            insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                            hasInDate = true;
                                        }
                                        else
                                        {
                                            if (checkInRateDate(tempRantStart, lstSubScript))
                                            {
                                                _scriptionRateWorkHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                            }
                                            else
                                            {
                                                normalHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                            }

                                            insSubScription(rantEnd, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, lstSubScript[i].WorkDayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                            lstSubScript[i].WorkDayHours = 0;
                                            hasInDate = true;
                                        }
                                    }

                                }
                                if (hasInDate)
                                {
                                    break;
                                }
                            }
                        }

                        if (hasInDate == false)
                        {


                            bool hasRate = checkInRateDate(tempRantStart, lstSubScript);
                            if (IsInHoliday(holidayList, tempRantStart))
                            {

                                holidayHour += tempPreHour;

                            }
                            else
                            {
                                if (hasRate)
                                {
                                    _scriptionRateWorkHour += tempPreHour;
                                }
                                else
                                {
                                    normalHour += tempPreHour;
                                }

                            }
                        }


                        break;
                    }
                    else
                    {
                        bool hasInDate1 = false;
                        bool hasInDate2 = false;

                        if (isFirstDay)
                        {
                            //檢查是否離跨日在15~45分之間，若有要算一小時
                            if (tempPreOriginHour == 0.5)
                            {
                                tempPreHour = 1;
                            }
                            if (tempPreHour > 0)
                            {
                                for (int i = 0; i < len; i++)
                                {
                                    if (IsInDate(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                                    {
                                        if (IsInHoliday(holidayList, tempRantStart))
                                        {
                                            if (lstSubScript[i].HolidayHours > 0)
                                            {
                                                if (lstSubScript[i].HolidayHours >= tempPreHour)
                                                {
                                                    holidayHour += 0;
                                                    lstSubScript[i].HolidayHours -= Convert.ToSingle(tempPreHour);
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    hasInDate1 = true;
                                                }
                                                else
                                                {

                                                    holidayHour += tempPreHour - lstSubScript[i].HolidayHours;
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, lstSubScript[i].HolidayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    lstSubScript[i].HolidayHours = 0;
                                                    hasInDate1 = true;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (lstSubScript[i].WorkDayHours > 0)
                                            {
                                                if (lstSubScript[i].WorkDayHours >= tempPreHour)
                                                {
                                                    normalHour += 0;
                                                    lstSubScript[i].WorkDayHours -= Convert.ToSingle(tempPreHour);
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    hasInDate1 = true;
                                                }
                                                else
                                                {
                                                    if (checkInRateDate(tempRantStart, lstSubScript))
                                                    {
                                                        _scriptionRateWorkHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                                    }
                                                    else
                                                    {
                                                        normalHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                                    }
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, lstSubScript[i].WorkDayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    lstSubScript[i].WorkDayHours = 0;
                                                    hasInDate1 = true;
                                                }
                                            }

                                        }
                                        if (hasInDate1)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tempPreHour > 0)
                            {
                                for (int i = 0; i < len; i++)
                                {
                                    if (IsInDate(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                                    {
                                        if (IsInHoliday(holidayList, tempRantStart))
                                        {
                                            if (lstSubScript[i].HolidayHours > 0)
                                            {
                                                if (lstSubScript[i].HolidayHours >= tempPreHour)
                                                {
                                                    holidayHour += 0;
                                                    lstSubScript[i].HolidayHours -= Convert.ToSingle(tempPreHour);
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    hasInDate1 = true;
                                                }
                                                else
                                                {
                                                    holidayHour += tempPreHour - lstSubScript[i].HolidayHours;
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, lstSubScript[i].HolidayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    lstSubScript[i].HolidayHours = 0;
                                                    hasInDate1 = true;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (lstSubScript[i].WorkDayHours > 0)
                                            {
                                                if (lstSubScript[i].WorkDayHours >= tempPreHour)
                                                {
                                                    normalHour += 0;
                                                    lstSubScript[i].WorkDayHours -= Convert.ToSingle(tempPreHour);
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, tempPreHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    hasInDate1 = true;
                                                }
                                                else
                                                {
                                                    if (checkInRateDate(tempRantStart, lstSubScript))
                                                    {
                                                        _scriptionRateWorkHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                                    }
                                                    else
                                                    {
                                                        normalHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                                    }
                                                    // normalHour += tempPreHour - lstSubScript[i].WorkDayHours;
                                                    insSubScription(tempRantStart, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, lstSubScript[i].WorkDayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                    lstSubScript[i].WorkDayHours = 0;
                                                    hasInDate1 = true;
                                                }
                                            }

                                        }
                                        if (hasInDate1)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (hasInDate1 == false)
                        {

                            bool hasRate = checkInRateDate(tempRantStart, lstSubScript);
                            if (IsInHoliday(holidayList, tempRantStart))
                            {
                                holidayHour += tempPreHour;
                            }
                            else
                            {
                                if (hasRate)
                                {
                                    _scriptionRateWorkHour += tempPreHour;
                                }
                                else
                                {
                                    normalHour += tempPreHour;
                                }

                            }
                        }


                        //未大於十要算nextDate到rantEnd的時數
                        double tempLaterHour = 0;

                        if (rantEnd <= nextRantStart)
                        {
                            tempLaterHour = GetHour((rantEnd - nextDate), true);
                        }
                        else
                        {
                            tempLaterHour = GetHour((nextRantStart - nextDate), true);
                        }

                        if (isFirstDay)
                        {
                            if (tempPreOriginHour == 0.5)
                            {
                                tempLaterHour -= 0.5;
                            }

                            isFirstDay = false;
                        }

                        if (tempLaterHour > (10 - tempPreHour))
                        {
                            tempLaterHour = (10 - tempPreHour);
                        }
                        if (tempLaterHour > 0)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                if (IsInDate(nextDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate))
                                {
                                    if (IsInHoliday(holidayList, nextDate))
                                    {
                                        if (lstSubScript[i].HolidayHours > 0)
                                        {
                                            if (lstSubScript[i].HolidayHours >= tempLaterHour)
                                            {
                                                holidayHour += 0;
                                                lstSubScript[i].HolidayHours -= Convert.ToSingle(tempLaterHour);
                                                insSubScription(nextDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, tempLaterHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                hasInDate2 = true;
                                            }
                                            else
                                            {

                                                holidayHour += tempLaterHour - lstSubScript[i].HolidayHours;
                                                insSubScription(nextDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, true, lstSubScript[i].HolidayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                lstSubScript[i].HolidayHours = 0;
                                                hasInDate2 = true;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (lstSubScript[i].WorkDayHours > 0)
                                        {
                                            if (lstSubScript[i].WorkDayHours >= tempLaterHour)
                                            {
                                                normalHour += 0;
                                                lstSubScript[i].WorkDayHours -= Convert.ToSingle(tempLaterHour);
                                                insSubScription(nextDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, tempLaterHour, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                hasInDate2 = true;
                                            }
                                            else
                                            {
                                                if (checkInRateDate(nextDate, lstSubScript))
                                                {
                                                    _scriptionRateWorkHour += tempLaterHour - lstSubScript[i].WorkDayHours;
                                                }
                                                else
                                                {
                                                    normalHour += tempLaterHour - lstSubScript[i].WorkDayHours;
                                                }
                                                //normalHour += tempLaterHour - lstSubScript[i].WorkDayHours;
                                                insSubScription(nextDate, lstSubScript[i].StartDate, lstSubScript[i].EndDate, false, lstSubScript[i].WorkDayHours, lstSubScript[i].MonthlyRentId, ref ListUseMonthly);
                                                lstSubScript[i].WorkDayHours = 0;
                                                hasInDate2 = true;
                                            }
                                        }

                                    }
                                    if (hasInDate2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (hasInDate2 == false)
                        {

                            bool hasRate = checkInRateDate(nextDate, lstSubScript);
                            if (IsInHoliday(holidayList, nextDate))
                            {

                                holidayHour += tempLaterHour;
                            }
                            else
                            {
                                if (hasRate)
                                {
                                    _scriptionRateWorkHour += tempLaterHour;
                                }
                                else
                                {
                                    normalHour += tempLaterHour;
                                }

                            }
                        }



                    }

                    //還車時間小於等於開始時間
                    if (rantEnd <= nextRantStart)
                    {
                        break;
                    }
                    else
                    {
                        //還車時間大於開始時間加一天
                        tempRantStart = tempRantStart.AddDays(1);
                    }

                }



            }//end while
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="hasHour">是否有時數了(非第一個小時意思)</param>
        /// <returns></returns>
        public double GetHour(TimeSpan timespan, bool hasHour = false)
        {
            double totalHour = 0;

            totalHour += timespan.Days * 10;

            if (timespan.Hours >= 10)
            {
                totalHour += 10;
            }
            else
            {
                totalHour += timespan.Hours;

                if (timespan.Minutes > 45)
                {
                    totalHour++;
                }
                else if (timespan.Minutes >= 15 && timespan.Minutes <= 45)
                {
                    totalHour += 0.5;
                }

            }

            if (totalHour == 0 && timespan.TotalMinutes != 0)
            {
                if (!hasHour)
                {
                    totalHour = 1;
                }

            }

            if (totalHour < 1 && (!hasHour))
            {
                totalHour = 1;
            }

            return totalHour;
        }
        /// <summary>
        /// 是否屬於優惠費率內
        /// </summary>
        /// <param name="date"></param>
        /// <param name="lstSubScriptionRate"></param>
        /// <returns></returns>
        private bool checkInRateDate(DateTime date, List<MonthlyRentData> lstSubScriptionRate)
        {
            bool flag = false;
            int RateCount = lstSubScriptionRate.Count;
            for (int i = 0; i < RateCount; i++)
            {
                if (IsInDate(date, lstSubScriptionRate[i].StartDate, lstSubScriptionRate[i].EndDate))
                {
                    _scriptionRateHolidayHourPrice = lstSubScriptionRate[i].HoildayRateForCar;
                    _scriptionRateWorkHourPrice = lstSubScriptionRate[i].WorkDayRateForCar;
                    flag = true;
                    break;
                }
            }
            return flag;
        }
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
        /// <summary>
        /// 計算機車可折抵時數
        /// </summary>
        /// <param name="rentmins"></param>
        /// <returns></returns>
        public int GetMotorCanDiscountPoint(int rentmins)
        {
            int CanDiscountPoint = 0;

            //首日最多折199分鐘
            if (rentmins <= 199)
            {
                CanDiscountPoint = rentmins;
            }
            else if (rentmins <= 600)
            {
                CanDiscountPoint = 199;
            }
            else
            {
                int days = rentmins / 600;
                CanDiscountPoint = (days - 1) * 200 + 199 + ((rentmins % 600) >= 200 ? 200 : rentmins % 600);
            }

            return CanDiscountPoint;
        }

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
        {//note: discCompute
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

        /// <summary>
        /// 前n分鐘0元
        /// </summary>
        /// <param name="disc">前n分鐘</param>
        /// <param name="sour">sour</param>
        /// <returns></returns>
        public List<DayPayMins> befMinsFree(double disc, List<DayPayMins> sour)
        {
            double lastDisc = 0;//現在點數

            if (sour != null && sour.Count() > 0 )
            {
                lastDisc = disc;
                var re = objUti.Clone(sour);
                re = re.OrderBy(x => x.xDate).ThenBy(y=>y.xSTime).ThenByDescending(z => z.haveNext).ToList();
                double mins = re.Select(x => x.xMins).Sum();

                if (lastDisc > mins)//自動縮減
                    lastDisc = mins;//實際使用點數

                foreach(var x in re)
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

        /// <summary>
        /// 首24H折扣後(平日折扣,假日折扣,折扣後金額,平日剩餘分,假日剩餘分)
        /// </summary>
        /// <param name="priceNmin">平日價格(分)</param>
        /// <param name="priceHmin">假日價格(分)</param>
        /// <param name="dayBaseMins">基本分鐘數</param>
        /// <param name="dayBasePrice">基消</param>
        /// <param name="Discount">折扣</param>
        /// <param name="fList">首日列表</param>
        /// <param name="wDateType">平日DateType</param>
        /// <param name="hDateType">假日DateType</param>
        /// <returns></returns>
        /// <mark>2020-12-14 eason</mark>
        public Tuple<double, double, double, double, double> MotoF24HDiscCompute(
            double priceNmin, double priceHmin, double dayBaseMins,
            double dayBasePrice, double dayMaxPrice, double Discount,
            ref List<DayPayMins> fList,
            string wDateType,
            string hDateType,
            double fDayMaxMins
            )
        {//note: MotoF24HDiscCompute

            string funNm = "MotoF24HDiscCompute : ";
            //double dayMaxMins = 199;//首日最大分鐘
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

                if ((fdate.isFull24H) ||(!fdate.isFull24H && fList.Count()==2))
                    FdateType = sAll;
                else if (fdate.haveNext == 1 && fList.Count() == 1)
                    FdateType = sFir;
                else if (fdate.haveNext == 0 && fList.Count() == 1)
                    FdateType = sLas;
                else
                    throw new Exception(funNm+ "fList資料錯誤");

                if (FdateType == sAll)
                {                   
                    var useDisc01 = tmpUseDisc > fdate.xMins ? fdate.xMins : tmpUseDisc;//折扣自動縮減
                    var f01_over6 = fdate.xMins > dayBaseMins ? (fdate.xMins - dayBaseMins) : 0;//超過基本分鐘的部分                    

                    //使用6分內不可折抵
                    //if (Mins < 6)
                    //    useDisc01 = 0;

                    if (fdate.DateType == wDateType)
                    {
                        if (useDisc01 == 0)
                        {
                            wLastMins += fdate.xMins;
                            f24Pay += (fdate.xMins - dayBaseMins) * priceNmin + dayBasePrice;
                        }
                        else
                        {
                            if (useDisc01 >= dayBaseMins)
                            {
                                wLastMins += (fdate.xMins - useDisc01);
                                f24Pay += (fdate.xMins - useDisc01) * priceNmin;
                                fdate.xMins -= useDisc01;
                                fdate.useBaseMins = dayBaseMins;
                            }
                            else
                            {
                                //折扣小於基本分只能折扣超過基本分的部分
                                if (f01_over6 > 0)
                                {
                                    useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                    f24Pay += dayBasePrice + ((fdate.xMins - dayBaseMins) - useDisc01) * priceNmin;
                                    wLastMins += (fdate.xMins - useDisc01);
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
                            tmpUseDisc -= useDisc01;
                            f1_wDisc += useDisc01;
                        }
                    }
                    else if (fdate.DateType == hDateType)
                    {
                        if (useDisc01 == 0)
                        {
                            hLastMins += fdate.xMins;
                            f24Pay += (fdate.xMins - dayBaseMins) * priceHmin + dayBasePrice;
                        }
                        else
                        {
                            if (useDisc01 >= dayBaseMins)
                            {
                                hLastMins += (fdate.xMins - useDisc01);
                                f24Pay += (fdate.xMins - useDisc01) * priceHmin;
                                fdate.xMins -= useDisc01;
                                fdate.useBaseMins = dayBaseMins;
                            }
                            else
                            {
                                if (f01_over6 > 0)
                                {
                                    useDisc01 = useDisc01 > f01_over6 ? f01_over6 : useDisc01;
                                    //折扣小於基本分只能折扣超過基本分的部分
                                    f24Pay += dayBasePrice + ((fdate.xMins - dayBaseMins) - useDisc01) * priceHmin;
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
                            tmpUseDisc -= useDisc01;
                            f1_hDisc += useDisc01;
                        }
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
                else if(FdateType == sFir)
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
                else if(FdateType == sLas)
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

                UseDisc = f_wDisc + f_hDisc;//使用點數
                if (UseDisc >= fDayMaxMins)//使用點數超過上限
                    f24Pay = 0;
                else if (f24Pay > dayMaxPrice)
                    f24Pay = dayMaxPrice;//價格超過上限
                //else 
                //{
                //    //193*1.5+10 = 299.5不足300          
                //    f24Pay = f24Pay > 0 ? (f24Pay + 0.5) : 0;
                //}
            }

            //f24Pay = Convert.ToInt32(Math.Round(f24Pay, 0, MidpointRounding.AwayFromZero));

            return new Tuple<double, double, double, double, double>(f_wDisc, f_hDisc, f24Pay, wLastMins, hLastMins);
        }

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
        {//note: GetDateMark
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
                            var mf = mData.Where(y =>
                                string.Compare(x.HolidayDate, y.StartDate.ToString("yyyyMMdd")) >= 0 &&
                                string.Compare(x.HolidayDate, y.EndDate.ToString("yyyyMMdd")) <= 0
                              ).FirstOrDefault();

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
                re.AddRange(FromHoliday(hDays));//一般假日                
            }

            if (mData != null && mData.Count() > 0)//月租日期
                mData.ForEach(x => re = (FillDateType(x.StartDate, x.EndDate, x.MonthlyRentId.ToString(), re)));

            re = re.Where(x =>
                            string.Compare(x.xDate, sd.ToString("yyyyMMdd")) >= 0 &&
                            string.Compare(x.xDate, ed.ToString("yyyyMMdd")) <= 0).ToList();

            re = FillDate(sd, ed, re);
            re = re.OrderBy(x => x.xDate).ToList();

            return re;
        }

        /// <summary>
        /// 將指定時間類別給予GroupId及起訖
        /// </summary>
        /// <param name="dateTypes">時間類別</param>
        /// <param name="sour"></param>
        /// <returns></returns>
        /// <mark>2020-12-21 eason</mark>
        public List<DayPayMins> GetDateGroup(List<string> dateTypes, string grpFNamg, List<DayPayMins> sour)
        {//note: GetDateGroup
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
                if(fdate.haveNext == 1)
                {
                    if (sour.Count() >= 2)
                        sour[1].isF24H = true;
                }

                var fDaysOntTppe = sour.Where(x => x.isF24H && x.isFull24H).OrderByDescending(y=>y.haveNext).FirstOrDefault();
                if (fDaysOntTppe != null)
                {
                    fDaysOntTppe.xMins = fDaysOntTppe.xMins >= oriMins ? fMins : fDaysOntTppe.xMins;
                    re.Add(fDaysOntTppe);
                }

                var fDaysTwoTppe = sour.Where(x => x.isF24H && !x.isFull24H).ToList();
                if (fDaysTwoTppe != null && fDaysTwoTppe.Count() > 0)
                {
                    fDaysTwoTppe.ForEach(x => {
                        var fds = sour.Where(y => y.xSTime == x.xSTime && y.xETime == x.xETime)
                        .OrderByDescending(a => a.haveNext).ToList();
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
                sour.ForEach(x => {
                    var d = re.Where(y =>
                    y.xSTime == x.xSTime && y.xETime == x.xETime &&
                    y.isStart == x.isStart && y.isEnd == x.isEnd &&
                    y.haveNext == x.haveNext && y.DateType == x.DateType
                    ).FirstOrDefault();
                    if (d != null)
                        x = d;
                });
            }

            return sour;
        }

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
        {//note: FillDateType

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

        public DayPayMins FromHoliday(Holiday sour)
        {
            DayPayMins re = new DayPayMins();
            if (sour != null)
            {
                List<Holiday> hos = new List<Holiday>();
                hos.Add(sour);
                re = FromHoliday(hos).FirstOrDefault();
            }
            return re;
        }

        public int GetDisc(int sour, int unit)
        {
            int re = 0;
            if (sour > 0)
            {
                var xre = Convert.ToDouble(sour) % Convert.ToDouble(unit);
                re = Convert.ToInt32(Convert.ToDouble(sour) - xre);
            }

            return re;
        }

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
            string funNM = "GetTimePart : ";
            double days = 0;
            double hours = 0;
            double mins = 0;
            double dayBasMins = 0;
            double dayMaxMins = 0;
            var proTys = new List<int>() { 0, 3, 4 };            

            if (sd == null || ed == null || sd > ed)
                throw new Exception(funNM + "sd,ed 格式錯誤");

            if (!proTys.Any(x => x == ProjType))
                throw new Exception(funNM + "ProjType 錯誤");

            var vsd = Convert.ToDateTime(sd.ToString("yyyy-MM-dd HH:mm"));
            var ved = Convert.ToDateTime(ed.ToString("yyyy-MM-dd HH:mm"));
            var vMins = ved.Subtract(vsd).TotalMinutes;
            var vDays = ved.Subtract(vsd).TotalDays;

            if (ProjType == 4)
            {
                double firstDayMaxMins = 199;
                dayBasMins = 6;
                dayMaxMins = 200;

                if(vDays > 1)
                {
                    days += 1;
                    vsd = vsd.AddDays(1);//去除首日
                    var xre = GetMotoRangeMins(vsd, ved, dayBasMins, dayMaxMins, new List<Holiday>());
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
                else
                {
                    if(vMins >= 199)
                    {
                        days = 1;
                        hours = 0;
                        mins = 0;
                    }
                    else
                    {
                        var vre = GetTimePart(vMins, firstDayMaxMins);
                        days = vre.Item1;
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
                    if(vre != null)
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
    }

    /// <summary>
    /// 汽車月租回傳
    /// </summary>
    public class CarRentInfo
    {//note: CarRentInfo
        /// <summary>
        /// 未逾時租金
        /// </summary>
        public int RentInPay { get; set; } = 0;
        /// <summary>
        /// 未逾時總租用時數
        /// </summary>
        public int RentInMins { get; set; } = 0;
        /// <summary>
        ///未逾時一般時段租用時數(可折抵時數)
        /// </summary>
        public int DiscRentInMins { get; set; } = 0;
        /// <summary>
        /// 未逾時折抵後時數
        /// </summary>
        public int AfterDiscRentInMins { get; set; } = 0;
        /// <summary>
        /// 使用月租點數
        /// </summary>
        public List<MonthlyRentData> mFinal { get; set; }
        /// <summary>
        /// 使用折抵點數
        /// </summary>
        public int useDisc { get; set; } = 0;
        /// <summary>
        /// 使用月租折抵點數
        /// </summary>
        public double useMonthDisc { get; set; } = 0;
        /// <summary>
        /// 使用月租平日折扣
        /// </summary>
        public double useMonthDiscW { get; set; } = 0;
        /// <summary>
        /// 使用月租假日折扣
        /// </summary>
        public double useMonthDiscH { get; set; } = 0;
        /// <summary>
        /// 剩餘月租點數
        /// </summary>
        public double lastMonthDisc { get; set; } = 0;
    }

    public class DayPayMins
    {
        public string DateType { get; set; }//日期分類
        public string xDate { get; set; }//格式yyyyMMdd
        public string xSTime { get; set; }//開始時間:格式格式yyyyMMddHHmm
        public string xETime { get; set; }//結束時間:格式格式yyyyMMddHHmm
        public double xMins { get; set; }//當日付費分鐘
        public double xRate { get; set; }//費率,汽車(每小時),機車每分鐘
        public bool isMarkDay { get; set; }//是否為註記日, 平日,假日,月租平日,月租假日
        public int haveNext { get; set; } //計費區段是否有下一日,1(有),0(沒有)
        public int isStart { get; set; }//是否為時間區段起點,1是,0否
        public int isEnd { get; set; }//是否為時間區段結束,1是,0否
        public double useBaseMins { get; set; }//使用基本時間分鐘
        public string dayGroupId { get; set; }//時間區段id
        public double isGrpStar { get; set; }//是否為group起,1是,0否
        public double isGrpEnd { get; set; }//是否為group迄,1是,0否
        public bool isF24H { get; set; }//是否為首24H,1是,0否
        public bool isFull24H { get; set; }//是否為完整24H,1是,0否
    }

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
}