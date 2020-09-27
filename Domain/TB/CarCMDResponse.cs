using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 執行遠傳cmd回傳結果
    /// </summary>
    public class CarCMDResponse
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
    }
}
