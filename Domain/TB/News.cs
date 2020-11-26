using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class News
    {
        public int NewsID { set; get; }
        public string Title { set; get; }
        /// <summary>
        /// 推播訊息類型
        /// <para>0:一般訊息</para>
        /// <para>1:含url</para>
        /// </summary>
        public Int16 NewsType { set; get; }
        public Int16 NewsClass { set; get; }
        public string Content { set; get; }
        public string URL { set; get; }
        public DateTime SD { set; get; }
        public DateTime ED { set; get; }
        public string isTop { set; get; }
    }
}
