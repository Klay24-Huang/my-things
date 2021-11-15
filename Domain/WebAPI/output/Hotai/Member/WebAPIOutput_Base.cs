using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{

    public class WebAPIOutput_Base
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public int status { get; set; } = -1;

        /// <summary>
        /// 編號
        /// </summary>
        public string traceId { get; set; }

        /// <summary>
        /// 錯誤資訊
        /// </summary>
        public Dictionary<Object, Object> errors { get; set; }
    }

    public class errors 
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }
    }
}
