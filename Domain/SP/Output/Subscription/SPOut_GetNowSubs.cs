using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetNowSubs
    {
        public string ProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double WorkDayHours { get; set; }
        public double HolidayHours { get; set; }
        public double CarTotalHours { get; set; }
        public double MotoWorkDayMins { get; set; }
        public double MotoHolidayMins { get; set; }
        public double MotoTotalMins { get; set; }
        public double WorkDayRateForCar { get; set; }
        public double HoildayRateForCar { get; set; }
        public double WorkDayRateForMoto { get; set; }
        public double HoildayRateForMoto { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Mode { get; set; }
        public Int64 MonthlyRentId { get; set; }
        public int MonLvl { get; set; }
        public int MonType { get; set; }
        public int IsMoto { get; set; }
        public int IsMix { get; set; }
        public string MonProDisc { get; set; }
    }
}