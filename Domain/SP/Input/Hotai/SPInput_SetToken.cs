using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SPInput_SetToken : SPInput_Base
    {
        public string IDNO { get; set; }
        public string PRGName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
