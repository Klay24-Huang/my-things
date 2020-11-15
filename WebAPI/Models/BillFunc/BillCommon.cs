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
                MilagePrice = Convert.ToInt32(Math.Floor((((Days * 10) + Hours + (Minutes < 60 ? 1 : (Minutes/60))) * baseMil) * MilageDef));
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
        private void insSubScription(DateTime Date, DateTime StartDate, DateTime EndDate, bool isHoliday, double tmpHours, Int64 SubScriptionID,ref List<MonthlyRentData> UseMonthlyRentDatas)
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
        public int CalBillBySubScription(DateTime SD, DateTime ED, List<Holiday> holidayList,int Price,int PriceH, ref string errCode, ref List<MonthlyRentData> lstSubScript, ref List<MonthlyRentData> ListUseMonthly)
        {
            double normalHour = 0.0;
            double holidayHour = 0.0;
            double totalHour = 0.0;


            this.HolidayRantCarBySubScription(holidayList, SD, ED, ref normalHour, ref holidayHour, ref totalHour, ref lstSubScript,ref  ListUseMonthly);
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
            int  bill = normalAllBill + holidayAllBill + scriptRateH + scriptRateW;
            return bill;
        }
        public void HolidayRantCarBySubScription(List<Holiday> holidayList, DateTime rantStart, DateTime rantEnd, ref double normalHour, ref double holidayHour, ref double totalHour, ref List<MonthlyRentData> lstSubScript,ref List<MonthlyRentData> ListUseMonthly)
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
    }
}