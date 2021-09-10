using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_BuyNowAddMonth_Q01
    {
        public string IDNO { get; set; }
        public string MonProjId { get; set; }
        public int MonProPeroid { get; set; }
        public int ShortDays { get; set; }
        public Int64 LogID { get; set; }
    }
}
