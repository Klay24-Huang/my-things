using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class MonCardParam
    {
        public int MonId { get; set; }
        public double CarPrice { get; set; }
        public double CarMarkPrice { get; set; }
        public string CarTitle { get; set; }
        public string CardDisc { get; set; }
        public double CarTotalHours { get; set; }
        public double WorkDayHours { get; set; }
        public double HolidayHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double MotoWorkDayMins { get; set; }
        public double MotoHolidayMins { get; set; }
        public double WorkDayRateForCar { get; set; }
        public double HoildayRateForCar { get; set; }
        public double WorkDayRateForMoto { get; set; }
        public double HoildayRateForMoto { get; set; }
    }
}