using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 個人訊息
    /// </summary>
    public class GetNotification
    {
        /// <summary>
        /// 個人推播流水號
        /// </summary>
        public Int64 NotificationID { get; set; }
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
        public string URL { get; set; }
        /// <summary>
        /// 未讀0 / 已讀1 
        /// </summary>
        public int readFlg { get; set; }
    }
}