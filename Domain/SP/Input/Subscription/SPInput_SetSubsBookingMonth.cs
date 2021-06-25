using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SetSubsBookingMonth
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public Int64 OrderNo { get; set; }
        public Int64 MonthlyRentId { get; set; }
        /// <summary>
        /// 使用月租平日時數
        /// </summary>
        public double UseCarWDHours { get; set; } = 0;
        /// <summary>
        /// 使用月租假日時數
        /// </summary>
        public double UseCarHDHours { get; set; } = 0;
        /// <summary>
        /// 使用機車不分平假日時數
        /// </summary>
        public double UseMotoTotalMins { get; set; } = 0;
        /// <summary>
        /// 履保是否完成(0未執行1是2否)
        /// </summary>
        public int EscrowStatus { get; set; } = 0;
        public DateTime? SetNow { get; set; }
    }
}
