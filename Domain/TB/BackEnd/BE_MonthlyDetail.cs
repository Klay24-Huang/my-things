using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_MonthlyDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        public string lend_place { set; get; }
        /// <summary>
        /// 平日剩餘總時數
        /// </summary>
        public Single UseWorkDayHours { set; get; }
        /// <summary>
        /// 假日剩餘總時數
        /// </summary>
        public Single UseHolidayHours { set; get; }
        /// <summary>
        /// 機車剩餘總時數
        /// </summary>
        public Single UseMotoTotalHours { set; get; }
        /// <summary>
        /// 使用時間
        /// </summary>
        public DateTime MKTime { set; get; }
        public int SEQNO { set; get; }
        public string ProjID { set; get; }
        public string ProjNM { set; get; }

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
    }
}
