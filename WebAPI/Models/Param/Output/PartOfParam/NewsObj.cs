using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class NewsObj
    {
        /// <summary>
        /// 活動流水號
        /// </summary>
        public Int64 NewsID { get; set; }
        /// <summary>
        /// 是否是新訊息
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        //public Int16 IsNews { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        //public int IsEDM { get; set; }

        /// <summary>
        /// 1:和運 2:系統 3:主題活動 4:優惠訊息 5:成就通知 6:帳務通知 7:回饋金 8:時數通知
        /// 9:取消狀態 10:繳費通知 11:審核失敗
        /// </summary>
        public Int16 ActionType { get; set; }

        /// <summary>
        /// 推播時間 20210914 ADD BY ADAM REASON.時間改為字串格式
        /// </summary>
        public string PushTime { get; set; }

        /// <summary>
        /// 主旨
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 連結URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 是否置頂
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        //public int isTop { get; set; }
        /// <summary>
        /// 已讀1 / 未讀0
        /// </summary>
        public int readFlg { get; set; }
    }
}