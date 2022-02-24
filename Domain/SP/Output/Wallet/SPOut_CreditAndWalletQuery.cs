namespace Domain.SP.Output.Wallet
{
    public class SPOut_CreditAndWalletQuery : SPOutput_Base
    {
        /// <summary>
        /// 付費方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// </summary>
        public int PayMode { get; set; }

        /// <summary>
        /// 錢包狀態
        /// <para>1:未啟用</para>
        /// <para>2:啟用</para>
        /// <para>3:凍結</para>
        /// <para>4:註記刪除</para>
        /// </summary>
        public string WalletStatus { get; set; }

        /// <summary>
        /// 錢包餘額
        /// </summary>
        public int WalletAmout { get; set; }

        /// <summary>
        /// 發票寄送方式
        /// <para>1:捐贈</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int MEMSENDCD { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { get; set; }

        /// <summary>
        /// 手機條碼
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 是否同意自動儲值
        /// <para>0:不同意</para>
        /// <para>1:同意</para>
        /// </summary>
        public int AutoStored { get; set; }

        /// <summary>
        /// 機車預扣款金額
        /// </summary>
        public int MotorPreAmt { get; set; }
    }
}