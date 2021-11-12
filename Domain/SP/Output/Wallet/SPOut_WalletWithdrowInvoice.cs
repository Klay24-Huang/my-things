using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOut_WalletWithdrowInvoice
    {
        /// <summary>
        /// 執行結果(0成功,1失敗)
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 執行結果
        /// </summary>
        public string ErrorMsg { get; set; }

    }
}
