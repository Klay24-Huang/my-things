namespace Domain.Flow.CarRentCompute
{
    public class OBIZ_InCheck : BIZ_CRBase
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long longOrderNo { get; set; }

        /// <summary>
        /// 折抵時數
        /// </summary>
        public int Discount { set; get; }
    }
}