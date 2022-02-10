using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class IFN_HotaiPaymenyBase
    {
        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }
        /// <summary>
        /// 執行人員
        /// </summary>
        public string insUser { get; set; }

    }
}
