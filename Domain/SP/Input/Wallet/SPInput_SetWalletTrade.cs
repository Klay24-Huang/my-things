using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_SetWalletTrade
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// 受贈方
        /// </summary>
        public string IDNO_To { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal TradeAMT { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
