using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetMySubs
    {
        //public int MonType { get; set; }
        /// <summary>
        /// 月租專案代碼
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 月租總期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短期總天數,非短期則為0
        /// </summary>
        public int ShortDays { get; set; }
    }
}