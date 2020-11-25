using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_ECRefund
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public ECRefundOriRequestParams OriRequestParams { get; set; }
        public ECRefundResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
    public class ECRefundItem
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string NonRedeem { get; set; }
        public string NonPoint { get; set; }
    }

    public class ECRefundRequestParams
    {
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeTime { get; set; }
        public string TradeAmount { get; set; }
        public string CardToken { get; set; }
        public string TradeType { get; set; }
        public string OriMerchantTradeNo { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public List<ECRefundItem> Item { get; set; }
        public string ResultUrl { get; set; }
    }

    public class ECRefundOriRequestParams
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public ECRefundRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }

    public class ECRefundInstall
    {
        public string InstallPeriod { get; set; }
        public string InstallDownPay { get; set; }
        public string InstallPay { get; set; }
        public string InstallDownPayFee { get; set; }
        public string InstallPayFee { get; set; }
    }

    public class ECRefundRedeem
    {
        public string RedeemPt { get; set; }
        public string RedeemAmt { get; set; }
        public string PostRedeemPt { get; set; }
    }

    public class ECRefundItem2
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string NonRedeem { get; set; }
        public string NonPoint { get; set; }
    }

    public class ECRefundResultData
    {
        public string ApposId { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeTime { get; set; }
        public string CardToken { get; set; }
        public string TradeAmount { get; set; }
        public string OriMerchantTradeNo { get; set; }
        public string RetCode { get; set; }
        public string RetMsg { get; set; }
        public string ServiceTradeNo { get; set; }
        public string ServiceTradeDate { get; set; }
        public string ServiceTradeTime { get; set; }
        public string AvailableAmount { get; set; }
        public string TradeType { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string ServiceInfo1 { get; set; }
        public string ServiceInfo2 { get; set; }
        public string ServiceInfo3 { get; set; }
        public ECRefundInstall Install { get; set; }
        public ECRefundRedeem Redeem { get; set; }
        public List<ECRefundItem2> Item { get; set; }
        public string AuthIdResp { get; set; }
        public string CardNumber { get; set; }
        public string GatewayBankNo { get; set; }
        public string MemberId { get; set; }
        public string CoBranded { get; set; }
        public string CoBrandCardEventCode { get; set; }
        public string CoBrandCardStartDate { get; set; }
        public string CoBrandCardEndDate { get; set; }
        public string PaymentType { get; set; }
    }

    public class ECRefundResponseParams
    {
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public ECRefundResultData ResultData { get; set; }
    }
}
