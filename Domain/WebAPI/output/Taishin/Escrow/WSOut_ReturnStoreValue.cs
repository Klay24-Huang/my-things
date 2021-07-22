using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Escrow
{
    public class WSOut_ReturnStoreValue: WSOut_EscrowBase
    {
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代號
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 台新訂單編號(此筆交易的編號，非商店傳入的原始交易編號)
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// 退還金額
        /// </summary>
        public int RefundAmount { get; set; }
        /// <summary>
        /// 退還紅利點數
        /// </summary>
        public int RefundBonus { get; set; }
        /// <summary>
        /// 帳戶總餘額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 紅利點數
        /// </summary>
        public int Bonus { get; set; }
        /// <summary>
        /// 交易時間YYYYMMDDhhmmss
        /// </summary>
        public string TransDate { get; set; }
    }
}
