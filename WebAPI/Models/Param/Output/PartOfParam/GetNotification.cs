using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 個人訊息
    /// </summary>
    public class GetNotification
    {
        /// <summary>
        /// 0: 一般訊息, 1: 取車, 2:還車, 3取消
        /// </summary>
        public Int16 ActionType { set; get; }
        public string SendTime { set; get; }
        public string Title { set; get; }
        public string Message { set; get; }
        public string url { set; get; }
    }
}