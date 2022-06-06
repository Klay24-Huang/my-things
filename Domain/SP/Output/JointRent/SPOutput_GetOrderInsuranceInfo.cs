namespace Domain.SP.Output.JointRent
{
    public class SPOutput_GetOrderInsuranceInfo : SPOutput_Base
    {
        /// <summary>
        /// 可否使用安心服務
        /// </summary>
        public int Insurance { get; set; }

        /// <summary>
        /// 主承租人每小時安心服務價格
        /// </summary>
        public int MainInsurancePerHour { get; set; }

        /// <summary>
        /// 單一共同承租人每小時安心服務價格
        /// </summary>
        public int JointInsurancePerHour { get; set; }

        /// <summary>
        /// 共同承租提示訊息
        /// </summary>
        public string JointAlertMessage { get; set; }
        /// <summary>
        /// 機車安心服務低消分鐘數
        /// </summary>
        public float BaseInsuranceMinutes { get; set; }
        /// <summary>
        /// 機車安心服務低消金額
        /// </summary>
        public float BaseMotoRate { get; set; }
        /// <summary>
        /// 機車安心服務分鐘(幾分鐘算一次錢)
        /// </summary>
        public float InsuranceMotoMin { get; set; }
        /// <summary>
        /// 機車安心服務金額(一次算多少錢)
        /// </summary>
        public float InsuranceMotoRate { get; set; }
        /// <summary>
        /// 共同承租人數
        /// </summary>
        public int Partners { get; set; }
    }
}