using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetTBCode
    {
        public Int64 CodeId { get; set; }
        public string CodeNm { get; set; }
        public string CodeDisc { get; set; }
        public string CodeGroup { get; set; }
        public int Sort { get; set; }
        public string TBMap { get; set; }
        public string TBFieldMap { get; set; }
        public string MapCode { get; set; }
    }
}
