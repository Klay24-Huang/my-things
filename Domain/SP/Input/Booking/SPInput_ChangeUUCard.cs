namespace Domain.SP.Input.Booking
{
    public class SPInput_ChangeUUCard : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; }

        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        /// 遠傳車機token
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// 是否為興聯車機(0:否;1:是)
        /// </summary>
        public int IsCens { get; set; }

        /// <summary>
        /// 舊悠遊卡卡號
        /// </summary>
        public string OldCardNo { get; set; }

        /// <summary>
        /// 新悠遊卡卡號
        /// </summary>
        public string NewCardNo { get; set; }
    }
}