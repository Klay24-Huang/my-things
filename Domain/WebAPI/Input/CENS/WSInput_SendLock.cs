using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 2.4上解鎖
    /// </summary>
    public class WSInput_SendLock:WSInput_Base
    {
        /// <summary>
        /// 上解鎖指令
        /// <para>0:全車解鎖</para><para>1:全車上鎖</para><para>2:中控鎖解鎖</para><para>3:中控鎖上鎖</para><para>4:防盜鎖解鎖</para><para>5:防盜鎖上鎖</para>
        /// </summary>
        public int CMD { set; get; }
    }
}
