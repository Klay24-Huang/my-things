namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTransferCheck
    {
        /// <summary>
        /// 身分證-受贈人
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 手機號碼-受贈人
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { get; set; }
    }
}