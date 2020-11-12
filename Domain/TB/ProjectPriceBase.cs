using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class ProjectPriceBase
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }

        /// <summary>
        /// 專案平日價
        /// </summary>
        public int PRICE { set; get; }
        /// <summary>
        /// 專案假日價
        /// </summary>
        public int PRICE_H { set; get; }
        /// <summary>
        /// 安心服務每小時價格
        /// 20201110 ADD BY ADAM
        /// </summary>
        public int InsurancePerHours { set; get; }
    }
}
