using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class CreditCardBindListByEasyrent
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
        /// 替代性信用卡卡號或替代表銀行卡號
        /// </summary>
        public string CardToken { get; set; }
        /// <summary>
        /// 官網需要，IRENT沒有留
        /// </summary>
        public string CardHash { get; set; }
        /// <summary>
        /// 到期日 官網需要，IRENT沒有留
        /// </summary>
        public string ExpDate { get; set; }
        /// <summary>
        /// 卡片類型 官網需要，IRENT沒有留
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 順序
        /// </summary>
        public int IDX { get; set; }
    }
}