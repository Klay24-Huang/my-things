namespace Domain.SP.Output.Bill
{
    public class TradeCloseList
    {
        /// <summary>
        /// 關帳紀錄檔流水號
        /// </summary>
        public int CloseID { get; set; }

        /// <summary>
        /// 授權類別
        /// </summary>
        public int AuthType { get; set; }

        /// <summary>
        /// 關帳金額
        /// </summary>
        public int CloseAmout { get; set; }

        /// <summary>
        /// 可否關帳
        /// <para>0:不關</para>
        /// <para>1:要關</para>
        /// <para>2:退貨</para>
        /// </summary>
        public int ChkClose { get; set; }

        /// <summary>
        /// 退款金額
        /// </summary>
        public int RefundAmount { get; set; } = 0;
    }
}