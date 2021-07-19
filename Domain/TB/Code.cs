using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class Code
    {
        public int CodeId { get; set; }
        public string CodeNm { get; set; }
        public string CodeDisc { get; set; }
        public string CodeGroup { get; set; }
        public int Sort { get; set; }
        public int UseFlag { get; set; }
    }
}
