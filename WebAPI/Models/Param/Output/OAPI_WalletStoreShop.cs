using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 錢包儲值-超商條碼
    /// </summary>
    public class OAPI_WalletStoreShop :OAPI_WalletStoreBase
    {
        /// <summary>
        /// 繳費期限(距今+1天) ex: 2021/03/31 23:59:59
        /// </summary>
        public string PayDeadline { get; set; }

        /// <summary>
        /// 超商條碼1
        /// </summary>
        public string ShopBarCode1 { get; set; }

        /// <summary>
        /// 超商條碼2
        /// </summary>
        public string ShopBarCode2 { get; set; }

        /// <summary>
        /// 超商條碼3
        /// </summary>
        public string ShopBarCode3 { get; set; }

        /// <summary>
        /// barcode64
        /// </summary>
        public string Barcode64 { get; set; }


    }
}