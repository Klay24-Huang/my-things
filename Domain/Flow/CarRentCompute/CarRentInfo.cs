using Domain.TB;
using System.Collections.Generic;

namespace Domain.Flow.CarRentCompute
{
    /// <summary>
    /// 汽車月租回傳
    /// </summary>
    public class CarRentInfo
    {
        /// <summary>
        /// 未逾時租金
        /// </summary>
        public int RentInPay { get; set; } = 0;

        /// <summary>
        /// 未逾時總租用時數
        /// </summary>
        public int RentInMins { get; set; } = 0;

        /// <summary>
        /// 未逾時可折抵時數
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

        /// <summary>
        /// 使用標籤優惠分鐘數
        /// </summary>
        public int UseGiveMinute { get; set; }
    }
}