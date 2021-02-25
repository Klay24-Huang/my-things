using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class MonFreePoint
    {
        public Int64 MonFreePointId { get; set; }
        public Int64 MonthlyRentId { get; set; }
        public int MotoFreeType { get; set; }
        public double MotoTotalFreeMins { get; set; }
        public double MotoWorkDayFreeMins { get; set; }
        public double MotoHolidyDayFreeMins { get; set; }
        public int CarFreeType { get; set; }
        public double CarTotalFreeHours { get; set; }
        public double CarWorkDayFreeHours { get; set; }
        public double CarHolidayFreeHours { get; set; }
    }
}
