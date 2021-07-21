using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_MonthlyDetailForView: BE_MonthlyDetail
    {
        /// <summary>
        /// 汽車平日優惠費率分鐘數
        /// </summary>
        public string WorkDayRateForCarHours { get; set; }
        
        /// <summary>
        /// 汽車假日優惠費率分鐘數
        /// </summary>
        public string HolidayRateForCarHours { get; set; }
        /// <summary>
        /// 機車優惠費率分鐘數
        /// </summary>
        public string RateForMotorHours { get; set; }

        public BE_MonthlyDetailForView(BE_MonthlyDetail toCopy)
        {
            this.OrderNo = toCopy.OrderNo;
            this.IDNO = toCopy.IDNO;
            this.lend_place = toCopy.lend_place;
            this.UseWorkDayHours = toCopy.UseWorkDayHours;
            this.UseHolidayHours = toCopy.UseHolidayHours;
            this.UseMotoTotalHours = toCopy.UseMotoTotalHours;
            this.MKTime = toCopy.MKTime;
            this.SEQNO = toCopy.SEQNO;
            this.ProjID = toCopy.ProjID;
            this.ProjNM = toCopy.ProjNM;
            this.WorkDayRateForCarMins = toCopy.WorkDayRateForCarMins;
            this.HolidayRateForCarMins = toCopy.HolidayRateForCarMins;
            this.RateForMotorMins = toCopy.RateForMotorMins;
            this.WorkDayRateForCarHours = WorkDayRateForCarMins > 0
                ? (Convert.ToDecimal(WorkDayRateForCarMins) / 60).ToString("##.###")
                : "-";
            this.HolidayRateForCarHours = HolidayRateForCarMins > 0
                ? (Convert.ToDecimal(HolidayRateForCarMins) / 60).ToString("##.###")
                : "-";
            this.RateForMotorHours = RateForMotorMins > 0
                ? (Convert.ToDecimal(RateForMotorMins) / 60).ToString("##.###")
                : "-";
        }

        public BE_MonthlyDetailForView atob(BE_MonthlyDetail a)
        {
            BE_MonthlyDetailForView b = (BE_MonthlyDetailForView)a ; ;
            //b.IDNO = a.IDNO;


            return b;
        }

    }
}
