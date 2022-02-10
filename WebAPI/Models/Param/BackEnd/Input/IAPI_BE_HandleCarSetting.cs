using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class IAPI_BE_HandleCarSetting:IAPI_BE_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 隸屬據點代碼
        /// </summary>
        public string StationId { set; get; }
        /// <summary>
        /// 目前據點代碼
        /// </summary>
        public string nowStationId { set; get; }

    }
}