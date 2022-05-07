using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 優惠標籤
    /// </summary>
    public class DiscountLabel
    {
        /// <summary>
        /// 標籤類型
        /// </summary>
        public string LabelType { get; set; } = "";

        /// <summary>
        /// 分鐘數
        /// </summary>
        public int GiveMinute { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; } = "";
    }
}