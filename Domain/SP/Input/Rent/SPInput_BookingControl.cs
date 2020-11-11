using System;

namespace Domain.SP.Input.Rent
{
    public class SPInput_BookingControl
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 執行的api log
        /// </summary>
        public Int64 LogID { get; set; }
    }
}