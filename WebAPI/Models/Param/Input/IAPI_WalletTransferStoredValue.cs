using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTransferStoredValue
    {
        /// <summary>
        /// 身分證或手機號碼(受贈人)
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { set; get; }
    }
}