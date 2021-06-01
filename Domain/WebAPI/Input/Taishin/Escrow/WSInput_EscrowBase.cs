using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Escrow
{
    public class WSInput_EscrowBase
    {
        /// <summary>
        /// API版本(目前版本0.1.01)
        /// </summary>
        public string ApiVersion { get; set; }
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代號
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// POS編號
        /// </summary>
        public string POSId { get; set; }
        /// <summary>
        /// 店家編號
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 店家交易時間YYYYMMDDhhmmss
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }
    }
}
