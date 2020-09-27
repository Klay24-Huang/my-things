using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Battle
{
    public class SPOutput_GetKey:SPOutput_Base
    {
        
        /// <summary>
        /// 產生的金鑰
        /// </summary>
        public string Key { set; get; }
    }
}
