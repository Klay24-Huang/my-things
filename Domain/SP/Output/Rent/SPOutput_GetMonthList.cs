using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
    public class SPOutput_GetMonthList
    {
        public List<SPOutput_GetMonthList_My> MyMonths { get; set; }
        public List<SPOutput_GetMonthList_Month> AllMonths { get; set; }
    }

    public class SPOutput_GetMonthList_My: SPOutput_GetMonthList_Month
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SPOutput_GetMonthList_Month
    {
        /// <summary>
        /// 方案代碼-key
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 方案名稱
        /// </summary>
        public string MonProjNM { get; set; }
        /// <summary>
        /// 總期數-key
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短期總天數,非短期則為0-key
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 方案價格
        /// </summary>
        public int PeriodPrice { get; set; }
        /// <summary>
        /// 注意事項
        /// </summary>
        public string MonProDisc { get; set; }
        /// <summary>
        /// 是否為機車0否1是
        /// </summary>
        public int IsMoto { get; set; }
        /// <summary>
        /// 優先序(數值越大越優先)
        /// </summary>
        public int MonLvl { get; set; }
        /// <summary>
        /// 月租分類0無1月租2訂閱制3短租
        /// </summary>
        public int MonType { get; set; }
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public int CarWDHours { get; set; }
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public int CarHDHours { get; set; }
        /// <summary>
        /// 汽車不分平假日時數
        /// </summary>
        public int CarTotalHours { get; set; }
        /// <summary>
        /// 機車平日時數
        /// </summary>
        public int MotoWDMins { get; set; }
        /// <summary>
        /// 機車假日時數
        /// </summary>
        public int MotoHDMins { get; set; }
        /// <summary>
        /// 機車不分平假日時數
        /// </summary>
        public int MotoTotalMins { get; set; }
        /// <summary>
        /// 方案起日-8碼
        /// </summary>
        public string SDATE { get; set; }
        /// <summary>
        /// 方案迄日-8碼
        /// </summary>
        public string EDATE { get; set; }
        /// <summary>
        /// 汽車平日優惠費率
        /// </summary>
        public double WDRateForCar { get; set; }
        /// <summary>
        /// 汽車假日優惠費率
        /// </summary>
        public double HDRateForCar { get; set; }
        /// <summary>
        /// 機車平日優惠費率
        /// </summary>
        public double WDRateForMoto { get; set; }
        /// <summary>
        /// 機車假日優惠費率
        /// </summary>
        public double HDRateForMoto { get; set; }
        /// <summary>
        /// 是否有訂閱
        /// </summary>
        public int IsOrder { get; set; }
        /// <summary>
        /// 是否為優惠方案
        /// </summary>
        public int IsDiscount { get; set; }
        /// <summary>
        /// 當期是否有繳費
        /// </summary>
        public int IsPay { get; set; }
    }

}
