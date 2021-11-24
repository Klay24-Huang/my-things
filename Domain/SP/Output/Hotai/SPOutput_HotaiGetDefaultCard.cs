using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Hotai
{
    public class SPOutput_HotaiGetDefaultCard : SPOutput_Base
    {
        public string HotaiCardID { get; set; }
        public string CardToken { get; set; }
       
    }
}
