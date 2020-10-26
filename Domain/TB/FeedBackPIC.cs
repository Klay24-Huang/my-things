using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
   public class FeedBackPIC
    {
        public Int64 FeedBackPICID { set; get; }
        /// <summary>
        /// 序號
        /// </summary>
        public Int16 SEQNO { set; get; }
        /// <summary>
        /// 圖片檔
        /// </summary>
        public string FeedBackFile { set; get; }
    }
}
