using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_HotaiTranStep3 : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 中信結果的頁面標題
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// 中信結果的頁面內容(解密前)
        /// </summary>
        public string PageContent { get; set; }
        public string PrgName { get; set; }
        public string PrgUser { get; set; }
    }
}
