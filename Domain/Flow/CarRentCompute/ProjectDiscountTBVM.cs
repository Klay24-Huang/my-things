namespace Domain.Flow.CarRentCompute
{
    public class ProjectDiscountTBVM
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProjID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CARTYPE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CUSTOMIZE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CUSDAY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DISTYPE { get; set; }//短整數

        /// <summary>
        /// 
        /// </summary>
        public double DISRATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double PRICE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double PRICE_H { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double DISCOUNT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double PHOURS { get; set; }
    }
}