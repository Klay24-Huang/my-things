using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.FET
{
    public class WebAPIOutput_Login
    {
        public string token { set; get; }
        public string refreshToken { set; get; }
    }
}
