using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsMonthByOrderNo
    {
        public string IDNO { get; set; }
        public Int64 OrderNo { get; set; }
        public string MonProjID { get; set; }
        public string MonProjNM { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public int IsMoto { get; set; }
        public int MonType { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
    }
}
