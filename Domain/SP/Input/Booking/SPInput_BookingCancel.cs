using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Booking
{
    /// <summary>
    /// 取消訂單
    /// </summary>
    public class SPInput_BookingCancel
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
        public string Token { set; get; }
        public Int64 LogID { set; get; }
        /// <summary>
        /// 自定義描述
        /// </summary>
        public string Descript { set; get; }
    }
}
