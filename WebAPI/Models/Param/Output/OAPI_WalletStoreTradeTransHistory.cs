using System.Collections.Generic;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletStoreTradeTransHistory
    {
        public List<OAPI_WalletStoreTradeTrans> TradeHis { get; set; }
    }

    public class OAPI_WalletStoreTradeTrans
    {
        /// <summary>
        /// 帳款流水號
        /// </summary>
        public int SEQNO { get; set; }

        /// <summary>
        /// 交易年分
        /// </summary>
        public int TradeYear { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string TradeDate { get; set; }

        /// <summary>
        /// 交易時間
        /// </summary>
        public string TradeTime { get; set; }

        /// <summary>
        /// 交易類別
        /// </summary>
        public string TradeTypeNm { get; set; }

        /// <summary>
        /// 交易類別註記
        /// </summary>
        public string TradeNote { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        public int TradeAMT { get; set; }

        /// <summary>
        /// APP上是否顯示：0:隱藏,1:顯示
        /// </summary>
        public int ShowFLG { get; set; }
    }
}