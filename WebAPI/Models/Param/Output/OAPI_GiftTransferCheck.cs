namespace WebAPI.Models.Param.Output
{
    public class OAPI_GiftTransferCheck
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 轉贈時數
        /// </summary>
        public int Amount { get; set; }
    }
}