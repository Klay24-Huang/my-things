using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Hotai
{
    public class SPOutput_MemberUnBind : SPOutput_Base
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

    }
}
