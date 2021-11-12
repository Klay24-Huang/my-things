using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletTransferCheck
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string PhoneNo { get; set; }
        public DateTime? SetNow { get; set; } = null;
    }
}
