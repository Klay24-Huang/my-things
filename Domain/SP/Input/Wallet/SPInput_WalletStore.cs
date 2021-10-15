using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletStore : SPInput_Base
    {
        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 錢包會員ID
        /// </summary>
        public string WalletMemberID { set; get; }

        /// <summary>
        /// 錢包虛擬ID
        /// </summary>
        public string WalletAccountID { set; get; }

        /// <summary>
        /// 台新交易編號
        /// </summary>
        public string TaishinNO { get; set; }

        /// <summary>
        /// 錢包狀態
        /// </summary>
        public int  Status { set; get; }

        /// <summary>
        /// 開戶時申請的mail
        /// </summary>
        public string  Email { set; get; }

        /// <summary>
        /// 開戶時申請的電話
        /// </summary>
        public string  PhoneNo { set; get; }

        /// <summary>
        /// 儲值金額
        /// </summary>
        public int StoreAmount { set; get; }

        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int WalletBalance { set; get; }

        /// <summary>
        /// 開戶時間
        /// </summary>
        public DateTime  CreateDate { set; get; }

        /// <summary>
        /// 最近一筆交易時間
        /// </summary>
        public DateTime  LastTransDate { set; get; }

        /// <summary>
        /// 最近一筆iRent儲值交易序號
        /// </summary>
        public string  LastStoreTransId { set; get; }

        /// <summary>
        /// 最近一筆台新交易序號
        /// </summary>
        public string  LastTransId { set; get; }

        /// <summary>
        /// 交易類別名稱
        /// </summary>
        public string TradeType { set; get; }

        /// <summary>
        /// 程式ID
        /// </summary>
        public string PRGID { set; get; }

        /// <summary>
        /// 交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款)
        /// </summary>
        public int Mode { set; get; }
        /// <summary>
        /// 來源站台(1:APP/2:Backend)
        /// </summary>
        public short InputSource { set; get; }

    }
}
