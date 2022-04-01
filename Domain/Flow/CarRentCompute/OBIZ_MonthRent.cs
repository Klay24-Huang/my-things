using Domain.TB;
using System.Collections.Generic;

namespace Domain.Flow.CarRentCompute
{
    public class OBIZ_MonthRent : OBIZ_CRBase
    {
        /// <summary>
        /// false:無月租;true:有月租
        /// </summary>
        public bool UseMonthMode { get; set; }

        /// <summary>
        /// 月租資訊
        /// </summary>
        public List<MonthlyRentData> monthlyRentDatas { get; set; }

        /// <summary>
        /// 是否為月租
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMonthRent { set; get; }

        /// <summary>
        /// 車資料
        /// </summary>
        public CarRentInfo carInfo { get; set; }

        /// <summary>
        /// 實際使用使用的折抵點數
        /// </summary>
        public int useDisc { get; set; }

        /// <summary>
        /// 車輛租金
        /// </summary>
        public int CarRental { set; get; }
    }
}