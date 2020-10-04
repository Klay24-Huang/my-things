using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTransferStoredValue
    {
        /// <summary>
        /// 受轉贈者身份證
        /// </summary>
        public string  TransID{set;get;}
        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { set; get; }
    }
}