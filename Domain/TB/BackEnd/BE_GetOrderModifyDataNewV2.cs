using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetOrderModifyDataNewV2: BE_GetOrderModifyDataNew
    {
        public string CardToken { set; get; }
        public string ArrearCardToken { set; get; }
        public string TaishinTradeNo { set; get; }
        public int ArrearAMT { set; get; }
        /// <summary>
        /// 機車基消
        /// </summary>
        public int BaseMinutes { set; get; }
        public string MerchantTradeNo { set; get; }
        /// <summary>
        /// 已退款金額
        /// </summary>
        public int RefundAmount { set; get; }
    }
}
