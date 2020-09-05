using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 資料更新列表
    /// </summary>
   public  class UPDList
    {
        /// <summary>
        /// 行政區最近更新時間
        /// </summary>
        public DateTime? AreaList { set; get; }
        /// <summary>
        /// 愛心捐贈碼最近更新時間
        /// </summary>
        public DateTime? LoveCode { set; get; }
        /// <summary>
        /// 同站最新更新時間
        /// </summary>
        public DateTime? NormalRent { set; get; }
        /// <summary>
        /// 電子欄欄最近更新時間
        /// </summary>
        public DateTime? Polygon { set; get; }
        /// <summary>
        /// 停車場最近更新時間
        /// </summary>
        public DateTime? Parking { set; get; }

    }
}
