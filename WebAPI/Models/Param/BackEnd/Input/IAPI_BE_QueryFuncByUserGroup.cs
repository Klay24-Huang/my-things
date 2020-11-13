﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_QueryFuncByUserGroup : IAPI_BE_Base
    {
        /// <summary>
        /// 使用者群組id
        /// </summary>
        public string UserGroupID { get; set; }
    }
}