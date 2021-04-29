﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetArrsSubsList
    {
        public SPOut_GetArrsSubsList_Date DateRange { get; set; }
        public List<SPOut_GetArrsSubsList_Card> Arrs { get; set; }
    }

    public class SPOut_GetArrsSubsList_Date
    {
        public string SD { get; set; }
        public string ED { get; set; }
    }

    public class SPOut_GetArrsSubsList_Card
    {
        public string MonProjNM { get; set; }
        public string ProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public int rw { get; set; }
        public int PeriodPayPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
