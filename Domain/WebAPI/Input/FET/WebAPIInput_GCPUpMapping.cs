using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.FET
{
    public class WebAPIInput_GCPUpMapping
    {
        public string deviceCID { set; get; }
        public string deviceName { set; get; }
        public string deviceToken { set; get; }
        public string deviceType { set; get; }
    }
}
