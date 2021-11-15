using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member.Param
{
    /// <summary>
    /// API請求參數
    /// </summary>
    public class ApiRequestParam
    {
        public string BaseUrl { get; set; }

        public string ApiUrl { get; set; }

        /// <summary>
        /// 未加密JSON
        /// </summary>
        public string JsonString { get; set; }

        public HttpWebRequest Request { get; set; }

       
        public RequestBody Body { get; set; }

        /// <summary>
        /// 程式名稱
        /// </summary>
        public string FunName { get; set; }

        /// <summary>
        /// 是否解密
        /// </summary>
        public bool DoDecrypt { get; set; }

        public bool SendRequest { get; set; } = true;

    }

    /// <summary>
    /// 傳送Body內容
    /// </summary>
    public class RequestBody
    {
        public string Body { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }

    }
}
