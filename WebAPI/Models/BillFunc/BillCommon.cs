using Domain.SP.Input.Bill;
using Domain.SP.Output.Bill;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
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
        /// 計算以分計費金額(不分平假日)
        /// </summary>
        /// <param name="days">幾天</param>
        /// <param name="hours">幾小時</param>
        /// <param name="minutes">幾分</param>
        /// <param name="baseMinutes">基本時數</param>
        /// <param name="BasePrice">基本費</param>
        /// <param name="price">平日每分鐘</param>
        /// <param name="priceH">假日每分鐘</param>
        /// <param name="MaxPrice">當日最高價</param>
        public void CalFinalPriceByMinutes(int days, int hours, int minutes, int baseMinutes, int BasePrice, float price, float priceH, int MaxPrice, ref int TotalPrice)
        {

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
        /// 計算里程費
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="MilageBase">由db取出的每公里費用(-1代表未有設定）</param>
        /// <param name="MilageDef">預設每公里費用</param>
        /// <param name="baseMil">每小時幾公里</param>
        /// <returns></returns>
        public int CalMilagePay(DateTime SD, DateTime ED, float MilageBase, float MilageDef, float baseMil)
        {
            int Days = 0, Hours = 0, Minutes = 0;
            int MilagePrice = 0;
            CalDayHourMin(SD, ED, ref Days, ref Hours, ref Minutes);
            if (MilageBase < 0)
            {
                MilagePrice = Convert.ToInt32(Math.Floor((((Days * 10) + Hours + (Minutes < 60 ? 1 : (Minutes / 60))) * baseMil) * MilageDef));
            }
            else
            {
                MilagePrice = Convert.ToInt32(Math.Floor((((Days * 10) + Hours + (Minutes < 60 ? 1 : (Minutes / 60))) * baseMil) * MilageBase));
            }
            return MilagePrice;
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
        public int CarRentCompute(DateTime SD, DateTime ED, int Price, int PriceH, double dayMaxHour, List<Holiday> lstHoliday, bool overTime = false)
        {
            int re = 0;
            var minsPro = new MinsProcess(GetCarPayMins);
            var dayPro = overTime ? new DayMinsProcess(CarOverTimeMinsToPayMins) : null;
            double baseMinutes = 60;

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
                MilagePrice = Convert.ToInt32(Math.Floor((TotalHour * baseMil) * MilageDef));
            }
            else
            {
                MilagePrice = Convert.ToInt32(Math.Floor((TotalHour * baseMil) * MilageBase));
            }
            return MilagePrice;
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


        public Tuple<double, double> GetCarRangeMins(DateTime SD, DateTime ED, double baseMinutes, double dayMaxMins, List<Holiday> lstHoliday)
        {
            var minsPro = new MinsProcess(GetCarPayMins);
            return GetRangeMins(SD, ED, baseMinutes, dayMaxMins, lstHoliday, minsPro);          
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
    }
}