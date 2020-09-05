using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Common
{
    public class SPOutput_RefrashToken:SPOutput_Base
    {
        /// <summary>
        /// 存取token
        /// </summary>
        public string Access_Token { set; get; }
        /// <summary>
        /// refrash token
        /// </summary>
        public string Refrash_Token { set; get; }
    }
}
