using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_BookingStart
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        public string UserID { set; get; }
        /// <summary>
        /// 路邊租還才能更改結束日
        /// </summary>
        public string StopTime { set; get; }
        /// <summary>
        /// 取車里程
        /// </summary>
        public Single NowMileage { set; get; } = 0;
        /// <summary>
        /// 執行的api log
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
