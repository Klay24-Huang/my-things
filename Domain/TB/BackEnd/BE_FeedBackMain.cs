using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_FeedBackMain
    {
        public Int64 FeedBackID { set; get; }
        public string IDNO { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:取車</para>
        /// <para>1:還車</para>
        /// <para>2:意見回饋</para>
        /// </summary>
        public Int16 mode { set; get; }
        /// <summary>
        /// 回饋訊息
        /// </summary>
        public string descript { set; get; }
        public string PIC1 { set; get; }
        public string PIC2 { set; get; }
        public string PIC3 { set; get; }
        public string PIC4 { set; get; }
        /// <summary>
        /// 處理訊息
        /// </summary>
        public string handleDescript { set; get; }
        /// <summary>
        /// 是否處理
        /// <para>0:未處理</para>
        /// <para>1:已處理</para>
        /// </summary>
        public Int16 isHandle { set; get; }
        public string opt { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME { set; get; }
        /// <summary>
        /// 手機
        /// </summary>
        public string MEMTEL { set; get; }
        public DateTime MKTime { set; get; }
    }
}
