﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetMonSetInfo
    {
        public int MonSetID { get; set; }
        public string MonProjID { get; set; }
        public string MonProjNM { get; set; }
        public int MonProPeriod { get; set; }
        public int PeriodPrice { get; set; }
        public int ShortDays { get; set; }
        public string MonProDisc { get; set; }
        public int IsMoto { get; set; }
        public int MonLvl { get; set; }
        public int MonType { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double CarTotalHours { get; set; }
        public double MotoWDMins { get; set; }
        public double MotoHDMins { get; set; }
        public double MotoTotalMins { get; set; }
        public string SDATE { get; set; }
        public string EDATE { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public int IsDiscount { get; set; }
    }
}