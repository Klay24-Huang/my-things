using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.ResultData
{
   public  class AuthResultData
    {
        public string HppUrl { get; set; }
        public string CardHash { get; set; }
        public string MemberId { get; set; }
        public string CoBranded { get; set; }
        public string CoBrandCardEventCode { get; set; }
        public string CoBrandCardStartDate { get; set; }
        public string CoBrandCardEndDate { get; set; }
        public string ApposId { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeTime { get; set; }
        public string CardToken { get; set; }
        public string TradeAmount { get; set; }
        public string PayAmount { get; set; }
        public string PaymentType { get; set; }
        public string RetCode { get; set; }
        public string RetMsg { get; set; }
        public string ServiceTradeNo { get; set; }
        public string ServiceTradeDate { get; set; }
        public string ServiceTradeTime { get; set; }
        public string TradeType { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string ServiceInfo1 { get; set; }
        public string ServiceInfo2 { get; set; }
        public string ServiceInfo3 { get; set; }
        public AuthInstall Install { get; set; }
        public AuthRedeem Redeem { get; set; }
        public string InvoiceMark { get; set; }
        public AuthCarrierN CarrierN { get; set; }
        public string NonRedeemAmt { get; set; }
        public string NonRedeemdescCode { get; set; }
        public string NonPointAmt { get; set; }
        public string NonPointDescCode { get; set; }
        public List<AuthItem> Item { get; set; }
        public string RTNPosActionCode { get; set; }
        public string RTNPosActionCodeMsg { get; set; }
        public string AuthIdResp { get; set; }
        public string CardNumber { get; set; }
        public string GatewayBankNo { get; set; }
        public string AvailableAmount { get; set; }
    }
}
