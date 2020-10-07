using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class AuthRequestParams
    {
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeTime { get; set; }
        public string TradeAmount { get; set; }
        public string CardToken { get; set; }
        public string TradeType { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string InstallPeriod { get; set; }
        public string InvoiceMark { get; set; }
        public string UseRedeem { get; set; }
        public string NonRedeemAmt { get; set; }
        public string NonRedeemdescCode { get; set; }
        public List<AuthItem> Item { get; set; }
        public string ResultUrl { get; set; }
    }
}
