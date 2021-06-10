using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsEscrowPay
    {
        public string IDNO { get; set; }
        public Int64 OrderNo { get; set; }
        public string AccountId { get; set; }
        public double Amount { get; set; }
    }
}
