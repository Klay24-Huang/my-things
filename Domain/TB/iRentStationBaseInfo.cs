using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 取出據點基本資料（名字及代碼）
    /// </summary>
    public class iRentStationBaseInfo
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 據點名稱，對應tb裡的Location
        /// </summary>
        public string StationName { set; get; }
    }
}
