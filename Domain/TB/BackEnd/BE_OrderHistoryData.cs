using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_OrderHistoryData
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNum { set; get; }
        /// <summary>
        /// 動作描述
        /// </summary>
        public string Descript { set; get; }
        /// <summary>
        /// 執行時間
        /// </summary>
        public DateTime MKTime { set; get; }
    }
}
