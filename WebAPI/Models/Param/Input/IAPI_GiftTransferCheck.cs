namespace WebAPI.Models.Param.Input
{
    public class IAPI_GiftTransferCheck
    {
        /// <summary>
        /// 身分證
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { get; set; }
    }
}