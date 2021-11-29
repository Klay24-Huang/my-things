using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_AddHotaiCards
    {
        public HotaiResAddCard ResponseData { get; set; }

        public HotaiResReqJsonPwd PostData { get; set; }

        public string GotoUrl { get; set; }

        public bool Succ { get; set; }

        //public string ServiceName { get; set; }

        //public string TXN { get; set; }

        //public string MAC { get; set; }

        //public string PostData { get; set; }
    }
}
