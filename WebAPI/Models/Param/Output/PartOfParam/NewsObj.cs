using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class NewsObj
    {
        /// <summary>
        /// 是否是新訊息
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsNews { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int IsEDM { get; set; }

        /// <summary>
        /// 1:和運 2:系統 3:活動 4:優惠
        /// </summary>
        public Int16 ActionType { get; set; }

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
        public int isTop { get; set; }
    }
}