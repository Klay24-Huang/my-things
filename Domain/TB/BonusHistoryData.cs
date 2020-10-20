using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class BonusHistoryData
    {
        /// <summary>
        /// 使用時間
        /// </summary>
        public string USEDATE { set; get; }
        /// <summary>
        /// 明細說明
        /// </summary>
        public string MEMO { set; get; }
        /// <summary>
        /// 點數
        /// </summary>
        public string GIFTPOINT { set; get; }
    }
}
