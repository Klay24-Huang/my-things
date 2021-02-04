using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 個人訊息
    /// </summary>
    public class GetNotification
    {
        /// <summary>
        /// 推播時間
        /// </summary>
        public DateTime PushTime { get; set; }

        /// <summary>
        /// 主旨
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 內文
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 連結URL
        /// </summary>
        public string url { get; set; }
    }
}