using System;

namespace Domain.TB
{
    public class EstimateData
    {
        /// <summary>
        /// 專案代號
        /// </summary>
        public string ProjID { get; set; }

        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { get; set; }

        /// <summary>
        /// 平日價
        /// </summary>
        public int WeekdayPrice { get; set; }

        /// <summary>
        /// 假日價
        /// </summary>
        public int HoildayPrice { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarTypeGroupCode { get; set; }

        /// <summary>
        /// 加購安心服務
        /// </summary>
        public int Insurance { get; set; }

        /// <summary>
        /// 每小時安心服務價格
        /// </summary>
        public int InsurancePerHours { get; set; }

        /// <summary>
        /// 起始時間
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime ED { get; set; }     
    }
}