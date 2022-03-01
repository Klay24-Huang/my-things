using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletReturn
    {
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; }

        /// <summary>
        /// 錢包虛擬ID
        /// </summary>
        public string WalletAccountID { get; set; }

        /// <summary>
        /// TB_OrderAuthReturn 識別欄位
        /// </summary>
        public long AuthSeq { get; set; }

        /// <summary>
        /// 退款金額
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 錢包餘額
        /// </summary>
        public int? WalletBalance { get; set; } = 0;

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TransDate { get; set; }

        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }

        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 交易類別名稱
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款;5:欠費繳交)
        /// </summary>
        public short Mode { get; set; }

        /// <summary>
        /// 交易狀態
        /// </summary>
        public int AuthFlg { get; set; }


        /// <summary>
        /// 交易回傳代碼
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 交易回傳訊息
        /// </summary>
        public string AuthMessage { get; set; }


        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }

        /// <summary>
        /// 交易是否重複
        /// </summary>
        public int IsDuplicate { get; set; }
    }
}
