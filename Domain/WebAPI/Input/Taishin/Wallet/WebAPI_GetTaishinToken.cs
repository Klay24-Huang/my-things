using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    public class WebAPI_GetTaishinToken
    {
        public string grant_type { get; set; } = "client_credentials";

        public string data { get; set; }

        public string encKey { get; set; }
    }
}
