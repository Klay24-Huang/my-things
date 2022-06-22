namespace Domain.SP.Output.Bill
{
    public class SPOutput_GetEstimateMotor_Q01
    {
        /// <summary>
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { get; set; }

        /// <summary>
        /// 基本消費
        /// </summary>
        public int BaseMinutesPrice { get; set; }

        /// <summary>
        /// 平日-每分鐘費率
        /// </summary>
        public double MinuteOfPrice { get; set; }

        /// <summary>
        /// 假日-每分鐘費率
        /// </summary>
        public double MinuteOfPriceH { get; set; }

        /// <summary>
        /// 單日上限金額
        /// </summary>
        public int MaxPrice { get; set; }

        /// <summary>
        /// 首日免費分鐘數
        /// </summary>
        public double FirstFreeMins { get; set; }

        /// <summary>
        /// 機車安心服務基消價格
        /// </summary>
        public double BaseMotoRate { get; set; }

        /// <summary>
        /// 機車安心服務分鐘(幾分鐘算一次錢)
        /// </summary>
        public double InsuranceMotoMin { get; set; }

        /// <summary>
        /// 機車安心服務金額(計算1次多少錢)
        /// </summary>
        public double InsuranceMotoRate { get; set; }
    }
}