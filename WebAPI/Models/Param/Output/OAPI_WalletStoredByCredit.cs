using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletStoredByCredit :OAPI_WalletStoreBase
    {

        /// <summary>
        /// 儲值時間
        /// </summary>
        public string Timestamp { get; set; }
    }
}