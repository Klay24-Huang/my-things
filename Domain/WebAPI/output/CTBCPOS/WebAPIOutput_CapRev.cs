using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.CTBCPOS
{
    /// <summary>
    /// 取消請款回覆結果
    /// </summary>
    public class WebAPIOutput_CapRev : WebAPIOutput_CTBCBase
    {

        /// <summary>
        /// 請款金額
        /// </summary>
        public int CapAmt { get; set; }

    }
}


