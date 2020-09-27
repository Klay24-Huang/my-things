using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Booking
{
    /// <summary>
    /// 讀到卡，綁定至會員
    /// </summary>
    public class SPInput_BindUUCard
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 取到的卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// 執行此筆的api log
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
