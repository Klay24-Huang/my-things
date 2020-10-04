using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoredMoney
    {
        /// <summary>
        /// 要儲值的金額
        /// </summary>
        public int Amount { set; get; }
    }
}