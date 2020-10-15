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
        /// 
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string StationId { set; get; }
    }
}