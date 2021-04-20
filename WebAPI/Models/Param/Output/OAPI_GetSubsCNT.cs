using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetSubsCNT
    {
        public OAPI_GetSubsCNT_Card NowCard { get; set; }
        public OAPI_GetSubsCNT_Card NxtCard { get; set; }
    }

    public class OAPI_GetSubsCNT_Card
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public string SD { get; set; }
        public string ED { get; set; }
    }

}