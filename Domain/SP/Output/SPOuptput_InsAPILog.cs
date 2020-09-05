using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output
{
    /// <summary>
    /// 執行usp_InsAPILog回傳的值
    /// </summary>
    public class SPOuptput_InsAPILog:SPOutput_Base
    {
        /// <summary>
        /// 回傳的LogID
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
