using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    /// <summary>
    /// 退貨
    /// </summary>
    public class WebAPIInput_Refund : WebAPIInput_CTBCBase
    {

        /// <summary>
        /// 請款金額
        /// </summary>
        public int CapAmt { get; set; } = 0;

        /// <summary>
        /// 退貨金額
        /// </summary>
        public int RefundAmt { get; set; } = 0;

        /// <summary>
        /// 請款批次編號
        /// </summary>
        public int CapBatchId { get; set; } = 0;

        /// <summary>
        /// 請款批次序號
        /// </summary>
        public int CapBatchSeq { get; set; } = 0;

    }
}
