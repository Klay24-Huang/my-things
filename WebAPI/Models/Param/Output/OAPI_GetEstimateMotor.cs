using Domain.TB;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetEstimateMotor
    {
        /// <summary>
        /// 租金
        /// </summary>
        public int CarRentBill { get; set; }

        /// <summary>
        /// 安心服務基本分鐘
        /// </summary>
        public int InsuranceBaseMinute { get; set; }

        /// <summary>
        /// 安心服務基本費用
        /// </summary>
        public double InsuranceBaseRate { get; set; }

        /// <summary>
        /// 機車安心服務分鐘(幾分鐘算一次錢)
        /// </summary>
        public double InsuranceMotoMin { get; set; }

        /// <summary>
        /// 機車安心服務金額(計算1次多少錢)
        /// </summary>
        public double InsuranceMotoRate { get; set; }

        /// <summary>
        /// 安心服務費用
        /// </summary>
        public int InsuranceBill { get; set; }

        /// <summary>
        /// 總計
        /// </summary>
        public int TotalBill { get; set; }

        /// <summary>
        /// 優惠標籤
        /// </summary>
        public EstimateDiscountLabel DiscountLabel { get; set; } = new EstimateDiscountLabel();
    }
}