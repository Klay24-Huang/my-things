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
        /// 請款批次ID(TB_HotaiTransaction CapBatchId)
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// 請款批次序號(TB_HotaiTransaction CapBatchSeq)
        /// </summary>
        public int BatchSeq { get; set; }
    }
}
