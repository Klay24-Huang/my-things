using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CENS_ReadCardInfo
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// gps時間
        /// </summary>
        public DateTime GPSTime { set; get; }
    }
}