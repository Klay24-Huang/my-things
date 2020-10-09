using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Mochi
{
    public partial class WebAPIOutput_MochiLogin
    {
        public Data data { get; set; }
    }

    public partial class Data
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public long expires_in { get; set; }
        public string scopes { get; set; }
    }
}
