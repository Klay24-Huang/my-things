using System;
using System.Collections.Generic;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetProject
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }

        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SDate { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string EDate { set; get; }

        /// <summary>
        /// 顯示方式
        /// <para>0:依據點代碼(此時據點代碼為必填)</para>
        /// <para>1:依經緯度（此時經緯度與半徑為必填)</para>
        /// </summary>
        public int? Mode { set; get; }

        /// <summary>
        /// 緯度
        /// </summary>
        public double? Latitude { set; get; }

        /// <summary>
        /// 經度
        /// </summary>
        public double? Longitude { set; get; }

        /// <summary>
        /// 半徑
        /// </summary>
        public double? Radius { set; get; }

        /// <summary>
        /// 是否使用安心服務
        /// </summary>
        public int Insurance { set; get; }

        /// <summary>
        /// 車款
        /// </summary>
        public string CarType { set; get; } //20210412 ADD BY ADAM REASON.目前搜尋api有可能兩邊都會丟

        /// <summary>
        /// 車款
        /// </summary>
        public List<string> CarTypes { get; set; } = new List<string>();

        /// <summary>
        /// 用車行程
        /// 1 = 個人身分，2 = 企業身分
        /// </summary>
        public Int16 CarTrip { get; set; } = 1;
    }
}