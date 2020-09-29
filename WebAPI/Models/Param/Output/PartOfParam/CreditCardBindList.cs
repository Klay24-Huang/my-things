using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class CreditCardBindList
    {
        /// <summary>
        /// 銀行帳號，當綁定為銀行帳戶時才有值
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// 信用卡卡號
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 信用卡自訂名稱
        /// </summary>
        public string CardName { get; set; }
  /// <summary>
  /// 剩餘額度
  /// </summary>
        public string AvailableAmount { get; set; }
        /// <summary>
        /// 替代性信用卡卡號或替代表銀行卡號
        /// </summary>
        public string CardToken { get; set; }
    }
}