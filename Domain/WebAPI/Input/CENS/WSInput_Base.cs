using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 興聯車機base input
    /// <para>2.1、2.5是直接使用，其他需繼承</para>
    /// </summary>
    public class WSInput_Base
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
    }
}
