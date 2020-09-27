using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Common
{
    public class SPInput_InsReadCard
    {
        public string CID { set; get; }
        public string CardNo { set; get; }
        public DateTime GPSTime { set; get; }
        public string Status { set; get; }
        public Int64 LogID { set; get; }
    }
}
