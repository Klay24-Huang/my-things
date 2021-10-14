using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletWithdrowInvoice
    {
        public int SEQNO { set; get; }
        public string INV_NO { set; get; }
        public string INV_DATE { set; get; }
        public string INV_ADDR { set; get; }
        public string RNDCODE { set; get; }
        public Int64 LogID { get; set; }
    }
}
