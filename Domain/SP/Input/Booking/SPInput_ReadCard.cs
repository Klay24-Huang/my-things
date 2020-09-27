using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Booking
{
    /// <summary>
    /// 綁定uu卡
    /// </summary>
    public class SPInput_ReadCard
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
        /// 執行的api log
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
