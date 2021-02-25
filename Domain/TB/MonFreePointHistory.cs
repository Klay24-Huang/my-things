using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class MonFreePointHistory
    {
        public Int64 Id { get; set; }
        public Int64 MonFreePointId { get; set; }
        public Int64 OrderNo { get; set; }
        public string IDNO { get; set; }
        public double UseMotoTotalFreeMins { get; set; }
        public double UseMotoWorkDayFreeMins { get; set; }
        public double UseMotoHolidyDayFreeMins { get; set; }
        public double UseCarTotalFreeHours { get; set; }
        public double UseCarWorkDayFreeHours { get; set; }
        public double UseCarHolidayFreeHours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
