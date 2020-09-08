using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 輸出目前server時間
    /// </summary>
    public class OAPI_GetServerTime
    {
        /// <summary>
        /// 取得server時間
        /// </summary>
        public string ServerTime { set; get; }
    }
}