using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 電池交換站資料
    /// </summary>
    public class BatExchangeStationData
    {
        /// <summary>
        /// 交換站名稱
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 交換站編號
        /// </summary>
        public string Station { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 總單位數
        /// </summary>
        public int TotalCnt { set; get; }
        /// <summary>
        /// 95%滿電電池數
        /// </summary>
        public int FullCnt { set; get; }
        /// <summary>
        /// 空槽數
        /// </summary>
        public int EmptyCnt { set; get; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { set; get; }
    }
}
