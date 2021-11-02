using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_OrderAuthReservation
    {

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 order_number { get; set; }
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 授權金額
        /// </summary>
        public int final_Price { get; set; }
        /// <summary>
        /// 預設卡片類型 0:和泰Pay 1:台新
        /// </summary>
        public int CardType { get; set; }
        /// <summary>
        /// 授權類型 (1 = 預約, 2 = 訂金, 4 = 延長用車, 3 = 取車, 5 = 逾時, 6 = 欠費, 7 = 還車)
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 排程通道
        /// </summary>
        public int GateNo { get; set; }
        /// <summary>
        /// 授權次數
        /// </summary>
        public int isRetry { get; set; }
        /// <summary>
        /// 是否自動關帳
        /// </summary>
        public int AutoClose { get; set; }
        /// <summary>
        /// 執行程式名稱
        /// </summary>
        public string PrgName { get; set; }
        /// <summary>
        /// 執行程式人員
        /// </summary>
        public string PrgUser { get; set; }
        /// <summary>
        /// 預約授權時間執行時間
        /// </summary>
        public DateTime AppointmentTime { get; set; }


    }
}
