﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_ReCalculateByDiscount:IAPI_BE_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        public int CarPoint { set; get; }
        public int MotorPoint { set; get; }
        public int ProjType { set; get; }
    }
}