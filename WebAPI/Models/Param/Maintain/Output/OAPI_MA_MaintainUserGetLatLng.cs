using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Output
{
    public class OAPI_MA_MaintainUserGetLatLng
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 管轄據點，以;分割
        /// </summary>
     //   public string StationGroup { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Lat { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Lng { set; get; }
    }
}