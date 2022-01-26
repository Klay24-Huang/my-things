using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class OFN_HotaiPaymentAuth
    {
        public string RtnCode { get; set; }
        public string CardNo { get; set; }
        public string Transaction_no { get; set; }
        /// <summary>
        /// 銀行端回傳的交易編號
        /// </summary>
        public string BankTradeNo { get; set; }
        /// <summary>
        /// 支付系統回傳的結果代碼
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 支付系統回傳的驗證訊息
        /// </summary>
        public string AuthMessage { get; set; }

        
        public int PreStep { get; set; }

    }
}
