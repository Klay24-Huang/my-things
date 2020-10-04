using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetParkingSpaceByReturn
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 停車格文字
        /// </summary>
        public string ParkingSpace { set; get; }
        /// <summary>
        /// 停車格圖片
        /// </summary>
        public string ParkingSpaceImage { set; get; }
    }
}