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
        public int MotoTotalMins { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public int IsDiscount { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM
        /// </summary>
        public int IsMix { get; set; }
    }

}