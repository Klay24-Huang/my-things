using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Bill
{
    /// <summary>
    /// 取里程
    /// </summary>
    public class SPOutput_GetMilageSetting:SPOutput_Base
    {
        /// <summary>
        /// 里程費
        /// </summary>
        public Single MilageBase { set; get; }
    }
}
