using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public  class HotaiTXNInfoForAdd
    {
        public string serviceName { get; set; }

        public string TXN { get; set; }

        public string MAC { get; set; }

        public string reqjsonpwd { get; set; }

    }
}
