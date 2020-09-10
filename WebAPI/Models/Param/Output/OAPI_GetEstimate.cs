using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetEstimate
    {
        /// <summary>
        /// 純租金
        /// </summary>
         public int CarRentBill { set; get; }
        /// <summary>
        /// 里程費
        /// </summary>
         public int MileageBill { set; get; }
        /// <summary>
        /// 每公里多少元
        /// </summary>
        public double MileagePerKM { set; get; }
        /// <summary>
        /// 安心服務每小時
        /// </summary>
        public int InsurancePerHour { set; get; }
        /// <summary>
        /// 安心服務費用
        /// </summary>
        public int InsuranceBill { set; get; }
        /// <summary>
        /// 總計
        /// </summary>
        public int Bill { set; get; }
    }
}