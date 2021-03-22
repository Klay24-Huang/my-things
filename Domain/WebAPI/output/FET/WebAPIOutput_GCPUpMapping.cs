using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.FET
{
    public class WebAPIOutput_GCPUpMapping
    {
        public string deviceCID { set; get; }
        public string deviceName { set; get; }
        public string upResult { set; get; }
        public string upTypeCode { set; get; }
    }
}
