using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class IFN_QueryDefaultCard : IFN_HotaiPaymenyBase
    {
        /// <summary>
        /// 會員帳號(身分證)
        /// </summary>
        public string IDNO { get; set; }
    }
}
