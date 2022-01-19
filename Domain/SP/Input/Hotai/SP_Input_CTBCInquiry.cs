using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_CTBCInquiry : SP_Input_CTBCCapBase
    {

        /// <summary>
        /// 中信交易編號
        /// </summary>
        public string Xid { get; set; } = "";


        /// <summary>
        /// 批次ID
        /// </summary>
        public int BatchId { get; set; } = 0;

        /// <summary>
        /// 批次序號
        /// </summary>
        public int BatchSeq { get; set; } = 0;

        /// <summary>
        /// 訂單目前狀態
        /// </summary>
        public int CurrentState { get; set; } = 0;


    }
}
