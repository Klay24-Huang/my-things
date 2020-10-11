using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class NewsObj
    {
        public string MessageKey { get; set; }
        /// <summary>
        /// 是否是新訊息
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsNew { set; get; } = 0;
        public int IsEDM { set; get; }
        /// <summary>
        /// 1:和運 2:系統 3:活動 4:優惠
        /// </summary>
        public Int16 ActionType { set; get; }

        public string Title { set; get; }
        public string Message { set; get; }
        public string URL { set; get; }
        /// <summary>
        /// 是否置頂
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsTop { get; set;}

        // [NewsID] AS MessageKey,0 AS IsNews, [Title], [Content], [URL], [NewsType], [NewsClass] AS ActionType, [SD], [ED], [isTop] 
    }
}