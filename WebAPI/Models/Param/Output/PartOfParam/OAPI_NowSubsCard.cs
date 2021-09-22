using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class OAPI_NowSubsCard
    {
        /// <summary>
        /// 月租編號
        /// </summary>
        public Int64 MonthlyRentId { get; set; }
        /// <summary>
        /// 月租方案代碼
        /// </summary>
        public string ProjID { get; set; }
        /// <summary>
        /// 總期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短期總天數
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 月租方案名稱
        /// </summary>
        public string MonProjNM { get; set; }
        /// <summary>
        /// 平日時數
        /// </summary>
        public double WorkDayHours { get; set; }
        /// <summary>
        /// 假日時數
        /// </summary>
        public double HolidayHours { get; set; }
        /// <summary>
        /// 機車分鐘數
        /// </summary>
        public double MotoTotalMins { get; set; }
        /// <summary>
        /// 汽車平日優惠費率
        /// </summary>
        public double WorkDayRateForCar { get; set; }
        /// <summary>
        /// 汽車假日優惠費率
        /// </summary>
        public double HoildayRateForCar { get; set; }
        /// <summary>
        /// 機車平日優惠費率
        /// </summary>
        public double WorkDayRateForMoto { get; set; }
        /// <summary>
        /// 機車假日優惠費率
        /// </summary>
        public double HoildayRateForMoto { get; set; }
        /// <summary>
        /// 月租起日
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 月租迄日
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}