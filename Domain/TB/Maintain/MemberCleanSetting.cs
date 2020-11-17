using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Maintain
{
    /// <summary>
    /// 整備人員資料
    /// </summary>
    public class MemberCleanSetting
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 管轄據點，以;分割
        /// </summary>
        public string StationGroup { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Lat { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Lng { set; get; }
    }
}
