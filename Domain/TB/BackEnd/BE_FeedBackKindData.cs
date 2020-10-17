using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_FeedBackKindData
    {
        /// <summary>
        /// pk
        /// </summary>
        public int FeedBackKindId { set; get; } 
        /// <summary>
        /// 星星數
        /// </summary>
        public int Star { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string  Descript { set; get; }
        /// <summary>
        /// 是否啟用
        /// <para>0:停用</para>
        /// <para>1:啟用</para>
        /// </summary>
        public Int16 use_flag { set; get; }
    }
}
