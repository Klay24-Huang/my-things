using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsBookingMonth
    {
        public Int64 SubsBookingMonthID { get; set; }
        public Int64 OrderNo { get; set; }
        public Int64 MonthlyRentId { get; set; }
    }
}
