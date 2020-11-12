using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.FET
{
    /// <summary>
    /// 接收執行命令結果
    /// </summary>
    public class SPInput_InsReceiveCMD
    {
        /// <summary>
        /// Key值
        /// </summary>
        public string requestId { set; get; }
        /// <summary>
        /// 執行的function
        /// </summary>
        public string method { set; get; }
        /// <summary>
        /// 執行結果
        /// </summary>
        public string CmdReply { set; get; }
        /// <summary>
        /// raw data
        /// </summary>
        public string receiveRawData { set; get; }
        public Int64 LogID { set; get; }
    }
}
