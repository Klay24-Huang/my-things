using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台-保有車輛設定
    /// </summary>
    public class BE_CarSettingData
    {
        /// <summary>
        /// 目前所在據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 目前所在據點名稱
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
    }
}
