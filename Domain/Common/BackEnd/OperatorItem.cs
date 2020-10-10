using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.BackEnd
{
    /// <summary>
    /// 處理項目
    /// </summary>
    public class OperatorItem
    {
        /// <summary>
        /// 下拉式內容值
        /// </summary>
        public string OptValue { set; get; } = "";
        /// <summary>
        /// 下拉式顯示的文字
        /// </summary>
        public string OptText { set; get; } = "";
    }
}
