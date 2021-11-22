using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_FastAddHotaiCard
    {
        public HotaiResFastBind PostData { get; set; }

        public string GotoUrl { get; set; }

        public bool Succ { get; set; }

    }
}
