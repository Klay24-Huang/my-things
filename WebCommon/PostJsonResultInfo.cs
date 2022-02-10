using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    public class PostJsonResultInfo
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succ { get; set; } = false;
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public string ErrCode { get; set; } = "000000";
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// API回覆訊息
        /// </summary>
        public string ResponseData { get; set; } = "";
        /// <summary>
        /// ProtocolStatusCode
        /// </summary>
        public int ProtocolStatusCode { get; set; }


    }
}
