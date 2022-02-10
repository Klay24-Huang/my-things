using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 錢包儲值-虛擬帳號
    /// </summary>
    public class OAPI_WalletStoreVisualAccount : OAPI_WalletStoreBase
    {
        /// <summary>
        /// 繳費期限 ex: 2021/10/01 23:59
        /// </summary>
        public string PayDeadline { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 匯款帳號
        /// </summary>
        public string VirtualAccount { get; set; }

    }
}