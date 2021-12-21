using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_CTBCCap : SP_Input_CTBCCapBase
    {
        /// <summary>
        /// 中信後台的交易識別碼
        /// </summary>
        public string Xid {get;set;}

        /// <summary>
        /// 請款金額
        /// </summary>
        public int CapAmt { get; set; } = 0;

        /// <summary>
        /// 請款狀態
        /// </summary>
        public int CapStatus { get; set; } = -1;

        /// <summary>
        /// 請款回覆代碼
        /// </summary>
        public string CapErrCode { get; set; } = "";

        /// <summary>
        /// 請款錯誤訊息
        /// </summary>
        public string CapErrorDesc { get; set; } = "";

        /// <summary>
        /// 批次ID
        /// </summary>
        public int CapBatchId { get; set; } = 0;

        /// <summary>
        /// 批次序號
        /// </summary>
        public int CapBatchSeq { get; set; } = 0;

        /// <summary>
        /// 是否請款成功
        /// </summary>
        public int IsSuccess { get; set; }= 0;
    }
}
