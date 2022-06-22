namespace Domain.SP.Input.Bill
{
    public class SPInput_GetEstimateMotor_Q01 : SPInput_Base
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂閱制ID
        /// </summary>
        public int MonId { get; set; }
    }
}