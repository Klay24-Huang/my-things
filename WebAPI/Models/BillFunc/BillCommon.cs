using Domain.SP.Input.Bill;
using Domain.SP.Output.Bill;
using Domain.TB;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 計算時間
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="Days">天數</param>
        /// <param name="Hours">時數</param>
        /// <param name="Minutes">分數</param>
        public void CalDayHourMin(DateTime SD, DateTime ED,ref int Days,ref int Hours,ref int Minutes)
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
        public void CalPointerToDayHourMin(int point,ref int Days,ref int Hours,ref int Minutes)
        {
            Days= Convert.ToInt32(Math.Floor(Convert.ToDouble(point / 600)));
            Hours = Convert.ToInt32(Math.Floor(Convert.ToDouble(((point%600) / 60))));
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
        public void CalFinalPriceByMinutes(int days,int hours,int minutes,int baseMinutes,int BasePrice,float price,float priceH,int MaxPrice,ref int TotalPrice)
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
            if(TotalMinutes>0 && TotalMinutes <= BaseMinutes)   //如果時間大於0且小於基本時數，則以基本費計算
            {
                TotalPrice = BasePrice;
            }
            else if(TotalMinutes>0)
            {
                int days = 0,hours=0,mins=0;
                CalMinuteToDayHourMin(TotalMinutes-BaseMinutes, ref days, ref hours, ref mins); //取出天、時、分
                TotalPrice=Convert.ToInt32(Math.Floor((MaxPrice*days)+(((Price*60*hours)<MaxPrice)? (Price * 60 * hours) : MaxPrice)+(mins*Price)))+BaseMinutes;
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
        public int CalMilagePay(DateTime SD, DateTime ED, float MilageBase, float MilageDef,float baseMil)
        {
            int Days = 0, Hours = 0, Minutes = 0;
            int MilagePrice = 0;
            CalDayHourMin(SD, ED, ref Days, ref Hours, ref Minutes);
            if (MilageBase < 0)
            {
                MilagePrice = Convert.ToInt32(Math.Floor((((Days * 10) + Hours + Minutes) * baseMil) * MilageDef));
            }
            else
            {
                MilagePrice = Convert.ToInt32(Math.Floor((((Days * 10) + Hours + Minutes) * baseMil) * MilageBase));
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
        public float GetMilageBase(string ProjID,string CarType,DateTime SDate,DateTime EDate,Int64 LogID)
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
                string SPName= new ObjType().GetSPName(ObjType.SPType.GetMilageSetting);
                SQLHelper<SPInput_GetMilageSetting, SPOutput_GetMilageSetting> sqlHelp = new SQLHelper<SPInput_GetMilageSetting, SPOutput_GetMilageSetting>(WebApiApplication.connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                new CommonFunc().checkSQLResult(ref flag,SPOutput.Error,SPOutput.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    MilageBase = SPOutput.MilageBase;
                }
            }
            catch(Exception ex)
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