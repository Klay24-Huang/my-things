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
        public string MemberID { get; set; }
        public string AccountId { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreateDate { get; set; }
        public double Amount { get; set; }

    }
}
