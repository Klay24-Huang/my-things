using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_GetMonSetInfo
    {
        public Int64 LogID { get; set; } = 0;
        public string MonProjID { get; set; } = "";
        public int MonProPeriod { get; set; } = 0;
        public int ShortDays { get; set; } = 0;
    }
}
