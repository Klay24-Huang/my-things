using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_ReturnCar
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 手機的定位(緯度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLat { get; set; } = 0;
        /// <summary>
        /// 手機的定位(經度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLon { get; set; } = 0;
    }
}