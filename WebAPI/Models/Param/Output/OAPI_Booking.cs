using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 預約
    /// </summary>
    public class OAPI_Booking
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 最後取車時間
        /// </summary>
        public string LastPickTime { set; get; }
    }
}