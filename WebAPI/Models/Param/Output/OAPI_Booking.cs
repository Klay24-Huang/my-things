namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 預約
    /// </summary>
    public class OAPI_Booking
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 最後取車時間
        /// </summary>
        public string LastPickTime { get; set; }

        /// <summary>
        /// 錢包餘額不足通知
        /// <para>0:不顯示</para>
        /// <para>1:顯示</para>
        /// </summary>
        public int WalletNotice { get; set; }
    }
}