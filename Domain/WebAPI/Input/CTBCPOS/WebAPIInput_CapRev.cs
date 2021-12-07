using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    /// <summary>
    /// 取消請款
    /// </summary>
    public class WebAPIInput_CapRev : WebAPIInput_CTBCBase
    {

        /// <summary>
        /// 批次ID
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// 批次序號
        /// </summary>
        public int BatchSeq { get; set; }
    }
}
