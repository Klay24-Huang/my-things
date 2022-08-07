using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetNotificationActJoblist
    {
        public List<IAPI_SetNotificationActJob> InputData { get; set; }
    }
    public class IAPI_SetNotificationActJob
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        public string TITLE { get; set; }
        /// <summary>
        /// 推播類型
        /// </summary>
        public string NType { get; set; }
        /// <summary>
        /// 推播種類 1:一般 2:URL
        /// </summary>
        public int? MType { get; set; }
        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Message { get; set; }

    }
}