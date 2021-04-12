using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_RecieveTsac
    {
        /// <summary>
        /// 繳費記錄明細
        /// </summary>
        public RecieveTsacData row { set; get; }
    }

    public class RecieveTsacData
    {
        /// <summary>
        /// 交易序號
        /// </summary>
        public string TransactionNo { set; get; }
        /// <summary>
        /// 入帳日期
        /// </summary>
        public string TDATE { set; get; }
        /// <summary>
        /// 入帳/沖正
        /// </summary>
        public string SIGN { set; get; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public string AMT { set; get; }
        /// <summary>
        /// 虛擬帳號
        /// </summary>
        public string TRNACTNO { set; get; }
        /// <summary>
        /// 通路代碼
        /// </summary>
        public string TXNCODE { set; get; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public string SDATE { set; get; }
        /// <summary>
        /// 交易時間
        /// </summary>
        public string TIME { set; get; }
        /// <summary>
        /// 匯出銀行
        /// </summary>
        public string OUTBANK { set; get; }
        /// <summary>
        /// 匯出帳號
        /// </summary>
        public string OUTACTNO { set; get; }
    }
}