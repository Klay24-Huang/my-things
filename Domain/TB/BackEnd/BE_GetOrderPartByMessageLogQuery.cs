using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台訊息記錄查詢_條件是訂單用
    /// </summary>
    public class BE_GetOrderPartByMessageLogQuery
    {
        public Int64 OrderNo { set; get; }
        public string CarNo { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime start_time { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime stop_time { set; get; }
        /// <summary>
        /// 4:已取車;11:已按還車;15:過完還車金流;16:完成還車
        /// </summary>
        public int car_mgt_status { set; get; }
        /// <summary>
        /// 5代表完成還車，大於0代表延長用車
        /// </summary>
        public int booking_status { set; get; }
        /// <summary>
        /// 大於0代表已取消
        /// </summary>
        public int cancel_status { set; get; }
        /// <summary>
        /// 實際取車時間，1911-01-01 00:00:00代表未取車
        /// </summary>
        public DateTime final_start_time { set; get; }
        /// <summary>
        /// 實際還車時間，1911-01-01 00:00:00代表未點還車
        /// </summary>
        public DateTime final_stop_time { set; get; }
    }
}
