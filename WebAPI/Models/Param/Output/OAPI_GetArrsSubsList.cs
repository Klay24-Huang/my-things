﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetArrsSubsList
    {
        public int TotalArresPrice { get; set; }
        public List<OAPI_GetArrsSubsList_card> Cards { get; set; }
    }

    public class OAPI_GetArrsSubsList_card
    {
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string ProjNm { get; set; } = "";
        /// <summary>
        /// 車型對照圖 20210617 ADD BY ADAM 
        /// </summary>
        public string CarTypePic { get; set; }
        public List<OAPI_GetArrsSubsList_arrs> Arrs { get; set; }
    }

    public class OAPI_GetArrsSubsList_arrs
    {
        public int Period { get; set; }
        public int ArresPrice { get; set; }
    }

}