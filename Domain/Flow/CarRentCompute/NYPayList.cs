using System;

namespace Domain.Flow.CarRentCompute
{
    public class NYPayList
    {
        /// <summary>
        /// 
        /// </summary>
        public Int64 order_number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PAYDATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PAYAMT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RETURNAMT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NORDNO { get; set; }
    }
}