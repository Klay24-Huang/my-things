using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 點數歷程資料
    /// </summary>
    public class WebAPIOutput_NPR271QueryData
    {
        /// <summary>
        /// 發生時間
        /// </summary>
        public string PROCDT { get; set; }
        /// <summary>
        /// 時數流水號
        /// </summary>
        public int GIFTSEQNO { get; set; }
        /// <summary>
        /// 時數種類 01汽車/02機車
        /// </summary>
        public string GIFTTYPE { get; set; }
        /// <summary>
        /// 歷程種類 01購買/02使用/03贈送/04轉贈
        /// </summary>
        public string LOGTYPE { get; set; }
        /// <summary>
        /// 分鐘數
        /// </summary>
        public int GIFTPOINT { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { get; set; }
    }
}
