using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_InsEscrowHist
    {
        public string IDNO { get; set; }
        public string MemberID { get; set; }
        public string AccountID { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string PhoneNo { get; set; }
        /// <summary>
        /// 履保金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 履保總額
        /// </summary>
        public int TotalAmount { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 最後一筆iRent儲值交易序號
        /// </summary>
        public string LastStoreTransId { get; set; }
        /// <summary>
        /// 最後一筆台新交易序號
        /// </summary>
        public string LastTransId { get; set; }
        /// <summary>
        /// 最近一筆交易時間
        /// </summary>
        public DateTime LastTransDate { get; set; }
        /// <summary>
        /// 履保台新回傳狀態
        /// </summary>
        public string EcStatus { get; set; }
    }
}
