using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_InsMonthlyHistory:SPInput_Base
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
        public Int64 MonthlyRentId { set; get; }
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
    }
}
