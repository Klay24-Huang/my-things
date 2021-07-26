using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SetSubsNxt
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string NxtMonProjID { get; set; }
        public int NxtMonProPeriod { get; set; }
        public int NxtShortDays { get; set; }
        public int AutoSubs { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
