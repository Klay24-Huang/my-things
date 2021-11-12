using System.Collections.Generic;

namespace Domain.SP.Output.Bill
{
    public class PreAmountData
    {
        /// <summary>
        /// 訂單差額
        /// </summary>
        public int DiffAmount { get; set; }

        /// <summary>
        /// 預授權清單
        /// </summary>
        public List<TradeCloseList> TradeCloseList { get; set; }
    }
}
