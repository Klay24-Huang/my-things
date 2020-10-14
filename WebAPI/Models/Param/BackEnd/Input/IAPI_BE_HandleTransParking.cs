using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleTransParking:IAPI_BE_Base
    {
        /// <summary>
        /// tb pk
        /// </summary>
        public int ParkingID { set; get; }
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
        public string OpenTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public string CloseTime { set; get; }
       

    }
}