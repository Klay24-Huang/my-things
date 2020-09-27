using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_ConfirmBeforeGiving
    {
        /// <summary>
        /// 身份證號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 受贈者手機
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 贈送分鐘數
        /// </summary>
        public int TransMins { set; get; }
    }
}