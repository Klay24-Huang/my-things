using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BookingStartMotor
    {
        /// <summary>
        /// 藍芽device name
        /// </summary>
        public string BLEDEVICEID { set; get; }
        /// <summary>
        /// 藍芽密碼
        /// </summary>
        public string BLEDEVICEPWD { set; get; }
    }
}