using System;

namespace Domain.SP.Output.Wallet
{
    public class SPOut_GetWalletStoreTradeTransHistory
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SEQNO { get; set; }

        /// <summary>
        /// 交易類別(對應財務FMFLAG)
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 交易代號
        /// </summary>
        public string TradeKey { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code0 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// 若1表負項，0表正項
        /// </summary>
        public int Negative { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal TradeAMT { get; set; }

        /// <summary>
        /// 交易備註
        /// </summary>
        public string TradeNote { get; set; }

        /// <summary>
        /// APP上是否顯示：0:隱藏,1:顯示
        /// </summary>
        public int ShowFLG { get; set; }
    }
}