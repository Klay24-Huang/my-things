using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    /// <summary>
    /// 取消退貨
    /// </summary>
    public class WebAPIInput_RefundRev : WebAPIInput_CTBCBase
    {
        /// <summary>
        /// 批次ID
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// 批次序號
        /// </summary>
        public int BatchSeq { get; set; }

        /// <summary>
        /// 退貨金額
        /// </summary>
        public int RefundAmt { get; set; }

    }
}
