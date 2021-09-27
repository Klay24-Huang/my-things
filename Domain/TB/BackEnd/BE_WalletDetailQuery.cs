using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_WalletDetailQuery
    {
        public string TradeType { get; set; }
        public string TradeKey { get; set; }
        public DateTime TradeDate { get; set; }
        public decimal TradeAMT { get; set; }
        public string ProjNM { get; set; }
        public int MonProPeriod { get; set; }
    }
}
