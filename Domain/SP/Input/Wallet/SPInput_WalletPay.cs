using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletPay : SPInput_Base
    {
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; }
        /// <summary>
        /// 錢包會員ID
        /// </summary>
        public string WalletMemberID { set; get; }
        /// <summary>
        /// 錢包虛擬ID
        /// </summary>
        public string WalletAccountID { set; get; }
        /// <summary>
        /// 扣款金額
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int WalletBalance { set; get; }
        /// <summary>
        /// 交易時間
        /// </summary>
        public DateTime TransDate { get; set; }
        /// <summary>
        /// 儲值交易序號
        /// </summary>
        public string StoreTransId { set; get; }
        /// <summary>
        /// 台新交易序號
        /// </summary>
        public string TransId { set; get; }
        /// <summary>
        /// 交易類別名稱
        /// </summary>
        public string TradeType { set; get; }
        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { set; get; }
        /// <summary>
        /// 交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款;5:欠費繳交)
        /// </summary>
        public int Mode { set; get; }
       
    }
}
