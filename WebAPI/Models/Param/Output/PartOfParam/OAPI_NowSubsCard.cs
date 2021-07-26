using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class OAPI_NowSubsCard
    {
        public Int64 MonthlyRentId { get; set; }
        public string ProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double WorkDayHours { get; set; }
        public double HolidayHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double WorkDayRateForCar { get; set; }
        public double HoildayRateForCar { get; set; }
        public double WorkDayRateForMoto { get; set; }
        public double HoildayRateForMoto { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}