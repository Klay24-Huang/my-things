using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoreTradeHistoryHidden
    {
        /// <summary>
        /// 組織代號(公司代碼)
        /// </summary>
        public string ORGID { get; set; }
        /// <summary>
        /// 帳款流水號(by會員)
        /// </summary>
        public int SEQNO { get; set; }
        /// <summary>
        /// 台新交易編號(IR編)
        /// </summary>
        public string TaishinNO { get; set; }

    }
}