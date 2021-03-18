using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_Banner:SPInput_Base
    {
        public int StationType { get; set; }
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }
        public string fileName1 { get; set; }
        public string URL { get; set; }
        public string RunHorse { get; set; }
        public string UserID { get; set; }
        public string SEQNO { get; set; }
    }
}
