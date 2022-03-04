using Domain.SP.Output.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR420Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { get; set; }

        public List<WalletTransferData> NPR420SavePayments { get; set; }
    }
}
