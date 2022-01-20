using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 月租資料（含優惠費率）
    /// </summary>
    public class MonthlyRentData
    {
        /// <summary>
        /// 類別
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int Mode {set;get;} 
        /// <summary>
        /// PK
        /// </summary>
        public Int64 MonthlyRentId { set; get; }
        /// <summary>
        /// 月租等級
        /// </summary>
        public int MonLvl { get; set; }
        /// <summary>
        /// 月租類別:0一般月租,1短期
        /// </summary>
        public int MonType { get; set; }
        /// <summary>
        /// 汽車免費時段: 0無,1平日,2假日,3不分平假日
        /// </summary>
        public int CarFreeType { get; set; }
        /// <summary>
        /// 機車免費時段: 0無,1平日,2假日,3不分平假日
        /// </summary>
        public int MotoFreeType { get; set; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 汽車剩餘總時數
        /// </summary>
        public double CarTotalHours { set; get; } = 0;
        /// <summary>
        /// 汽車平日剩餘時數
        /// </summary>
        public double WorkDayHours { set; get; } = 0;
        /// <summary>
        /// 汽車假日剩餘時數
        /// </summary>
        public double HolidayHours { set; get; } = 0;
        /// <summary>
        /// 機車剩餘總時數
        /// </summary>
        public double MotoTotalHours { set; get; } = 0;
        /// <summary>
        /// 機車平日剩餘分鐘
        /// </summary>
        public double MotoWorkDayMins { set; get; } = 0;
        /// <summary>
        /// 機車假日剩餘分鐘
        /// </summary>
        public double MotoHolidayMins { set; get; } = 0;
        /// <summary>
        /// 汽車平日優惠費率
        /// </summary>
        public double WorkDayRateForCar { set; get; } = 0;
        /// <summary>
        /// 汽車假日優惠費率
        /// </summary>
        public double HoildayRateForCar { set; get; } = 0;
        /// <summary>
        /// 機車平日優惠費率
        /// </summary>
        public double WorkDayRateForMoto { set; get; } = 0;
        /// <summary>
        /// 機車假日優惠費率
        /// </summary>
        public double HoildayRateForMoto { set; get; } = 0;
        /// <summary>
        /// 開始時間
        /// </summary>
         public DateTime StartDate { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { set; get; }
        /// <summary>
        /// 是否為城市車手
        /// </summary>
        public int IsMix { set; get; }
    }
}
