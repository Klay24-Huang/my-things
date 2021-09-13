using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_PersonNoticeRead
    {
        /// <summary>
        /// 個人推播流水號
        /// </summary>
        public Int64? NotificationID { get; set; } = 0;
    }
}