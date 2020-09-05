using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Common
{
    /// <summary>
    /// 寫入呼叫第三方api log
    /// </summary>
    public class SPInut_WebAPILog
    {
        /// <summary>
        /// api網址
        /// </summary>
        public string WebAPIURL { set; get; }
        /// <summary>
        /// api name
        /// </summary>
        public string WebAPIName { set; get; }
        /// <summary>
        /// 送出資料
        /// </summary>
        public string WebAPIInput { set; get; }
        /// <summary>
        /// 回傳資料
        /// </summary>
        public string WebAPIOutput { set; get; }
        /// <summary>
        /// 送出時間
        /// </summary>
        public DateTime MKTime { set; get; }
        /// <summary>
        /// 回傳時間
        /// </summary>
        public DateTime UPDTime { set; get; }
    }
}
