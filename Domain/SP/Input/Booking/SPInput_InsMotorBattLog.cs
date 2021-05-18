using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Booking
{
    /// <summary>
    /// 寫入機車電量紀錄
    /// </summary>
    public class SPInput_InsMotorBattLog
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 事件代碼  1:取車電量 2:還車電量 3:換電前 4:換電後
        /// </summary>
        public string EventCD { get; set; }
        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
    }
}
