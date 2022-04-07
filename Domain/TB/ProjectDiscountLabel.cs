using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class ProjectDiscountLabel: DiscountLabel
    {
        /// <summary>
        /// 折扣金額
        /// </summary>
        public string AppDescribe { get; set; } = "";
    }
}
