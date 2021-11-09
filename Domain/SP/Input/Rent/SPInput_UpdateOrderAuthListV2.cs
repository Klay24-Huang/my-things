using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_UpdateOrderAuthListV2
    {
        /// <summary>
        /// 授權流水號
        /// </summary>
        public Int64 authSeq { get; set; }
        /// <summary>
        /// 授權結果
        /// </summary>
		public int AuthFlg { get; set; }
        /// <summary>
        /// 授權回傳代碼
        /// </summary>
		public string AuthCode { get; set; }
        /// <summary>
        /// 授權回傳訊息
        /// </summary>
        public string AuthMessage { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 授權交易編號
        /// </summary>
        public string transaction_no { get; set; }
        /// <summary>
        /// 授權類型
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 是否為重試訂單(0:否 1:是)
        /// </summary>
        public int isRetry { get; set; }
        /// <summary>
        /// 會員帳號(身分證)
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 自動關帳(0:不關帳 1:關帳)
        /// </summary>
        public int AutoClosed { get; set; }
        /// <summary>
        /// 授權金額
        /// </summary>
        public int final_price { get; set; }
        /// <summary>
        /// 信用卡號
        /// </summary>
        public int CardNumber { get; set; }
        /// <summary>
        /// 執行程式名稱
        /// </summary>
        public string ProName { get; set; }

    }
}
