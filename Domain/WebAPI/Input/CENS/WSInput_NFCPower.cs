using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    public class WSInput_NFCPower : WSInput_Base
    {
        /// <summary>
        /// NFC電源控制
        /// <para>0:關閉</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int CMD { set; get; }
    }
}
