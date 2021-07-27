using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetMonthGroup
    {
        public string MonProjID { get; set; }
        /// <summary> 
        /// 20210616 ADD BY ADAM
        /// API模式 0:一般  1:變更期數
        /// </summary>
        public string Mode { get; set; }
    }
}