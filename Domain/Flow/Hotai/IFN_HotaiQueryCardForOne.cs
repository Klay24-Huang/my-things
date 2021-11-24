using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.Flow.Hotai
{
    public class IFN_HotaiQueryCardForOne: IFN_HotaiPaymenyBase
    {
        /// <summary>
        /// 會員帳號(身分證)
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 卡片識別碼
        /// </summary>
        public string CardToken { get; set; }
    }
}