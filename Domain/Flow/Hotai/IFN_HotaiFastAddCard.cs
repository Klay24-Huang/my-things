using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class IFN_HotaiFastAddCard : IFN_HotaiPaymenyBase
    {

        public string IDNO { get; set; }
        /// <summary>
        /// 中信卡友生日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 中信卡友身分證
        /// </summary>
        public string CTBCIDNO { get; set; }
        /// <summary>
        /// 接收回傳結果頁
        /// </summary>
        public string RedirectURL { get; set; }
    }
}
