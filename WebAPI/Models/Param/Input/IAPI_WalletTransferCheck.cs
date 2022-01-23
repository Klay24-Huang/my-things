namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTransferCheck
    {
        /// <summary>
        /// 身分證或手機號碼
        /// </summary>
        public string IDNO_Phone { get; set; } = "";

        /// <summary>
        /// 身分證-相容舊版
        /// </summary>
        public string IDNO { get; set; } = "";

        /// <summary>
        /// 轉贈金額-相容舊版
        /// </summary>
        public int Amount { get; set; } = 0;
    }
}