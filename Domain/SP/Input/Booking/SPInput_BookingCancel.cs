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
        /// 寫入OrderHistory說明文字
        /// </summary>
        public string Descript { set; get; }
        /// <summary>
        /// 回寫orderMain訂單取消狀態
        /// </summary>
        public string Cancel_Status_in { set; get; }
        /// <summary>
        /// 0為不檢查 1為需檢查
        /// </summary>
        public Int64 CheckToken { set; get; }
    }
}
