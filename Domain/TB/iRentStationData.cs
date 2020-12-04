using Domain.WebAPI.output.rootAPI;
using System.Collections.Generic;

namespace Domain.TB
{
    public class iRentStationData
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 據點名稱，對應tb裡的Location
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 電話
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 其他說明
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 據點照片
        /// </summary>
        public List<StationInfoObj> StationPic { get; set; }
    }
}