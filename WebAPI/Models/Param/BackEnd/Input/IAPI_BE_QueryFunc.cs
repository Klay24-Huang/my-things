using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_QueryFunc:IAPI_BE_Base
    {
        /// <summary>
        /// 功能群組id
        /// </summary>
        public string FuncGroupID { get; set; }
    }
}