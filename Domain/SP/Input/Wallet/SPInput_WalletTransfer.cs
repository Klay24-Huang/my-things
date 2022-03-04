using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    /// <summary>
    /// 錢包介面拋轉
    /// </summary>
    public class SPInput_WalletTransfer
    {
        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }

        /// <summary>
        /// 轉檔狀態 (Y:已轉檔 N:未轉檔 P:轉檔中 T:不需轉檔)
        /// </summary>
        public string F_TRFCOD { get; set; }


    }
}
