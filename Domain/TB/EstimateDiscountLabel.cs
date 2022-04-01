using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class EstimateDiscountLabel : DiscountLabel
    {
        /// <summary>
        /// 則扣金額
        /// </summary>
        public int Price { get; set; }

    }
}
