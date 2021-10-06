using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletWithdrowInvoice : IAPI_Base
    {
        /// <summary>
        /// 對應錢包主檔索引(外部索引直)
        /// </summary>
        public int SEQNO { set; get; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string INV_NO { set; get; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string INV_DATE { set; get; }
        /// <summary>
        /// 發票隨機碼
        /// </summary>
        public string RNDCODE { set; get; }
    }
}