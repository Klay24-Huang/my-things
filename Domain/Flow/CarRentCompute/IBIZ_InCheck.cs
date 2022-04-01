namespace Domain.Flow.CarRentCompute
{
    public class IBIZ_InCheck
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }


        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }

        /// <summary>
        /// 是否為訪客
        /// </summary>
        public bool isGuest { get; set; }

        /// <summary>
        /// 月租Id(可多筆)
        /// </summary>
        public string MonIds { get; set; }
    }
}