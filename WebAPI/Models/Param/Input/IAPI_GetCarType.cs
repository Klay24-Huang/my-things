using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetCarType
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string ED { set; get; }
    }
}