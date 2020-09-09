using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 計費公用程式
    /// </summary>
    public class BillCommon
    {
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
            //2017-01-24新增，避免尾差
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
    }
}