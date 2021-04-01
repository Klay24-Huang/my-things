using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Bill
{
    public class SPOut_GetBuyNowInfo
    {
        public Int64 CodeId { get; set; }
        public string CodeNm { get; set; }
        public string CodeDisc { get; set; }
        public string CodeGroup { get; set; }
        public int Sort { get; set; }
    }
}
