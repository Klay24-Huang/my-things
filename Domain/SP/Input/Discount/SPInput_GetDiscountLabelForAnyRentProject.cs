using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Discount
{
    public class SPInput_GetDiscountLabelForAnyRentProject : SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; } = "";
    }
}
