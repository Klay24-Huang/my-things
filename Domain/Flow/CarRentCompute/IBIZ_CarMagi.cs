using System;

namespace Domain.Flow.CarRentCompute
{
    public class IBIZ_CarMagi
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime ED { get; set; }

        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
    }
}