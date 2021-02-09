using System;

namespace Domain.TB
{
    public class GetFullProjectInfo
    {
        /// <summary>
        /// 優惠專案代碼
        /// </summary>
        public string PROJID { get; set; }

        /// <summary>
        /// 專案起日
        /// </summary>
        public DateTime ShowStart { get; set; }

        /// <summary>
        /// 專案迄日
        /// </summary>
        public DateTime ShowEnd { get; set; }

        /// <summary>
        /// 專案類型
        /// </summary>
        public string PROJTYPE { get; set; }

        /// <summary>
        /// 據點ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string CARTYPE { get; set; }

        /// <summary>
        /// 優惠價(平日)
        /// </summary>
        public int PRICE { get; set; }

        /// <summary>
        /// 優惠價(假日)
        /// </summary>
        public int PRICE_H { get; set; }

        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { get; set; }
    }
}