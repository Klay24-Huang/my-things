using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.FET
{
    /// <summary>
    /// 寫入呼叫記錄
    /// </summary>
    public class SPInput_InsSendCMD
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
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 遠傳提供的deviceToken
        /// </summary>
        public string deviceToken { set; get; }
        /// <summary>
        /// 傳出的參數
        /// </summary>
        public string SendParams { set; get; }
        /// <summary>
        /// http狀態碼
        /// </summary>
        public string HttpStatus { set; get; }
        public Int64 LogID { set; get; }
    }
}
