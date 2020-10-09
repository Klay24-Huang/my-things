using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Mochi
{
    public class SPOutput_GetMachiToken : SPOutput_Base
    {
        /// <summary>
        /// 車麻吉token
        /// </summary>
        public string Token { set; get; }
    }
}
