using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_FeedBack
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// <para>0:取車</para>
        /// <para>1:還車(這個才有star)</para>
        /// <para>2:用於關於我們(停用)</para>
        /// </summary>
        public int Mode { set; get; }
        public List<int> FeedBackKind { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { set; get; }
        /// <summary>
        /// 當mode=1時，值才有意義
        /// 當mode=0時，代入0
        /// </summary>
        public int Star { set; get; }
    }
}