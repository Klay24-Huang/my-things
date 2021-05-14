using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetArrsSubsList
    {
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string ProjNm { get; set; } = "";
        public List<OAPI_GetArrsSubsList_arrs> Arrs { get; set; }
    }

    public class OAPI_GetArrsSubsList_arrs
    {
        public int Period { get; set; }
        public int ArresPrice { get; set; }
    }

}