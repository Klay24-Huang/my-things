using System;

namespace Domain.SP.Output.Booking
{
    public class SPOutput_BookingCancel
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int Order_number { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string MEMIDNO { get; set; }

        /// <summary>
        /// 邀請人姓名
        /// </summary>
        public string MEMCNAME { get; set; }
    }
}