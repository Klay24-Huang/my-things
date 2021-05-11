using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SetSubsBookingMonth
    {
        public Int64 OrderNo { get; set; }
        public Int64 MonthlyRentId { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
