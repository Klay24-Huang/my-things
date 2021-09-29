using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoreBase
    {
        /// <summary>
        /// 儲值的金額
        /// </summary>
        public int StoreMoney { set; get; }
    }
}