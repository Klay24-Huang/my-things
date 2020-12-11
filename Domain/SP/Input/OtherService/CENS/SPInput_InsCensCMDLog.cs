using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.CENS
{
    /// <summary>
    /// 傳送接收執行命令結果
    /// </summary>
    public class SPInput_InsCensCMDLog
    {
        /// <summary>
        /// 執行的function
        /// </summary>
        public string Method { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 傳出的參數
        /// </summary>
        public string SendParams { set; get; }
        /// <summary>
        /// 接收資料
        /// </summary>
        public string ReceiveRawData { set; get; }
        /// <summary>
        /// 送出時間
        /// </summary>
        public DateTime MKTime { set; get; }
        /// <summary>
        /// 回傳時間
        /// </summary>
        public DateTime UPDTime { set; get; }
        public Int64 LogID { set; get; }
    }
}
