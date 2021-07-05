using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_InsertChargeParking
    {
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
        public string OpenTime { set; get; }
        public string CloseTime { set; get; }
        public string Id { set; get; }
    }
}