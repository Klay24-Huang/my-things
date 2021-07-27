using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_InsMonthlyHistory:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        public Int64 MonthlyRentId { set; get; }
        /// <summary>
        /// 汽車總時數
        /// </summary>
        public int UseCarTotalHours { get; set; }
        /// <summary>
        /// 使用的汽車平日時數
        /// </summary>
        public int UseWorkDayHours { set; get; }
        /// <summary>
        /// 使用的汽車假日時數
        /// </summary>
        public int UseHolidayHours { set; get; }
        /// <summary>
        /// 使用的機車時數
        /// </summary>
        public int UseMotoTotalHours { set; get; }
        /// <summary>
        /// 機車平日時數
        /// </summary>
        public int UseMotoWorkDayMins { get; set; }
        /// <summary>
        /// 機車假日時數
        /// </summary>
        public int UseMotoHolidayMins { get; set; }

    }
}
