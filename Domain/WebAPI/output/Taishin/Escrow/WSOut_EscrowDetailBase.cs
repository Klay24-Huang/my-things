using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Escrow
{
    public class WSOut_EscrowDetailBase
    {
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// 商戶代號
        /// </summary>
        public string MerchantId { get; set; }
    }
}
