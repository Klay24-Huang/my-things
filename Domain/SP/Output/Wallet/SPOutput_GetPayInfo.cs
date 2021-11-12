using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOutput_GetPayInfo
    {
        public int DefPayMode { get; set; }

        public int PayModeBindCount { get; set; }

        public string PayModeList { get; set; }
    }
}
    