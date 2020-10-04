using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
   public  class SPInput_HandleWallet:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
       public string IDNO { set; get; }
       /// <summary>
       /// 會員id
       /// </summary>
        public string  WalletMemberID { set; get; }
        /// <summary>
        /// 虛擬帳號id
        /// </summary>
        public string  WalletAccountID { set; get; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int  Status { set; get; }
        /// <summary>
        /// 開戶email
        /// </summary>
        public string  Email { set; get; }
        /// <summary>
        /// 開戶電話
        /// </summary>
        public string  PhoneNo { set; get; }
        /// <summary>
        /// 儲值金額
        /// </summary>
        public int  Amount { set; get; }
        /// <summary>
        /// 總額
        /// </summary>
        public int TotalAmount { set; get; }
        /// <summary>
        /// 開戶時間
        /// </summary>
        public DateTime  CreateDate { set; get; }
        /// <summary>
        /// 最近一筆交易時間
        /// </summary>
        public DateTime  LastTransDate { set; get; }
        /// <summary>
        /// 最後一筆iRent儲值交易序號
        /// </summary>
        public string  LastStoreTransId { set; get; }
        /// <summary>
        /// 最後一筆台新交易序號
        /// </summary>
        public string  LastTransId { set; get; }
        public string Token { set; get; }
    }
}
