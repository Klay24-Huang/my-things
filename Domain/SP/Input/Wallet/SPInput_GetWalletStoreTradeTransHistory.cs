using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetWalletStoreTradeTransHistory
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
    }
}
