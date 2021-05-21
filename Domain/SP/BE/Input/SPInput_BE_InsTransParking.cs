using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsTransParking : SPInput_Base
    {
        
        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string ParkingName { set; get; }
        /// <summary>
        /// 停車場地址
        /// </summary>
        public string ParkingAddress { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime OpenTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime CloseTime { set; get; }
        /// <summary>
        /// 操作者帳號
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// ID
        /// </summary>
        public string ID { set; get; }
    }
}
