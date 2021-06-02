using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 指定尋車方式
    /// </summary>
    public class WSInput_SearchCarForSituation : WSInput_Base
    {
        /// <summary>
        /// 尋車方式
        /// <para>0:同步鳴叫閃爍</para><para>1:喇叭鳴叫</para><para>2:方向燈閃爍</para>
        /// </summary>
        public int CMD { set; get; }
    }
}
