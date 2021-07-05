using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleTransParking_Moto
    {
        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string ParkingName { set; get; }
        /// <summary>
        /// 機車專用格數
        /// </summary>
        public string ParkingCNT { set; get; }
        /// <summary>
        /// 停車場地址
        /// </summary>
        public string ParkingAddress { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal ParkingLng { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal ParkingLat { set; get; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string OpenTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public string CloseTime { set; get; }
        /// <summary>
        /// 操作者帳號
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string ParkingMark { set; get; }
        /// <summary>
        /// ID
        /// </summary>
        public string ParkingID { set; get; }
    }
}