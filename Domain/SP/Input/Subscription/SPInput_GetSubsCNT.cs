using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_GetSubsCNT
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
