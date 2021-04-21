using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetChgSubsList
    {
        /// <summary>
        /// 目前訂閱
        /// </summary>
        public OPAI_GetChgSubsList_Card MyCard { get; set; }
        /// <summary>
        /// 其他方案
        /// </summary>
        public List<OPAI_GetChgSubsList_Card> OtrCards { get; set; }
    }

    public class OPAI_GetChgSubsList_Card
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public int PeriodPrice { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public int IsDiscount { get; set; }
    }

}