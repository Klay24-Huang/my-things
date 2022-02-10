using Domain.WebAPI.output.Hotai.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class OFN_HotaiFastAddCard
    {
        public HotaiResFastBind postData { get; set; }

        public string gotoUrl { get; set; }

        public bool succ { get; set; }


    }
}
