using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoreTradeTransHistory
    {
        /// <summary>
        /// 查詢起日(必填)
        /// </summary>
        public DateTime SD { get; set; }
        /// <summary>
        /// 查詢迄日(必填)
        /// </summary>
        public DateTime ED { get; set; }
    }
}