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
        /// APP優惠標籤說明
        /// </summary>
        public string AppDescribe { get; set; } = "";
    }
}
