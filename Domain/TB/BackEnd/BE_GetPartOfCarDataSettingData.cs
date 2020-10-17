using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台-車輛資料
    /// </summary>
    public class BE_GetPartOfCarDataSettingData
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 所屬據點
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 目前據點
        /// </summary>
        public string NowStationName { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 目前狀態
        /// <para>0:出租中</para>
        /// <para>1:可出租</para>
        /// <para>2:待上線</para>
        /// </summary>
        public Int16 NowStatus { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 所屬據點
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 目前據點
        /// </summary>
        public string nowStationID { set; get; }
    }
}
