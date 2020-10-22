using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_CarScheduleTimeLog:IAPI_BE_Base
    {
        /// <summary>
        /// 據點
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public string SD { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public string ED { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
    }
}