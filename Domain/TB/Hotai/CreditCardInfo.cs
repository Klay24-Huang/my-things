using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.TB
{
    /// <summary>
    /// 通用信用卡資訊
    /// </summary>
    public class CreditCardInfo
    {
        /// <summary>
        /// 信用卡Token
        /// </summary>
        public string CardToken { get; set; }
        /// <summary>
        /// 信用卡自訂名稱
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// 發卡機構(VISA/MASTER/JCB)
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 發卡銀行
        /// </summary>
        public string BankDesc { get; set; }
        /// <summary>
        /// 卡號(隱碼)
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 是否為預設卡(0:否/1:是)
        /// </summary>
        public int IsDefault { get; set; }

    }
}