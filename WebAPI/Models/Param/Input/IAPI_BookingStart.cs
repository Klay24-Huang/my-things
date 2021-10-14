using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 汽車取車
    /// </summary>
    public class IAPI_BookingStart
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 路邊租還可以重設結束時間
        /// </summary>
        public string ED { set; get; }
        /// <summary>
        /// SKB的token
        /// </summary>
        public string SKBToken { set; get; }

        /// <summary>
        /// 加購安心服務(0:否;1:有)
        /// </summary>
        public int Insurance { get; set; }
        /// <summary>
        /// 手機的定位(緯度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLat { get; set; } = 0;
        /// <summary>
        /// 手機的定位(經度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLon { get; set; } = 0;
    }
}