using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    /// <summary>
    /// 查詢訂單輸出
    /// </summary>
    public class WebAPIOutput_GetPaymentInfo
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public GetPaymentInfoOriRequestParams OriRequestParams { get; set; }
        public GetPaymentInfoResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
    public class GetPaymentInfoOriRequestParams {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public string TransNo { get; set; }
        public GetPaymentInfoRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
    public class GetPaymentInfoRequestParams
    {
        public string ServiceTradeNo { get; set; }
    }
    public class GetPaymentInfoResponseParams {
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public GetPaymentInfoResultData ResultData { get; set; }
    }
    public class GetPaymentInfoResultData {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeTime { get; set; }
        public string Tid { get; set; }
        public string Barcode { get; set; }
        public string Barcode2 { get; set; }
        public string Barcode3 { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public string TradeAmount { get; set; }
        /// <summary>
        /// 實際支付金額
        /// </summary>
        public string PayAmount { get; set; }
        public string RetCode { get; set; }
        public string RetMsg { get; set; }
        public string ServiceTradeNo { get; set; }
        public string ServiceTradeDate { get; set; }
        public string ServiceTradeTime { get; set; }
        /// <summary>
        /// 交易類別
        /// <para>1:授權</para>
        /// <para>5:退貨</para>
        /// <para>6:取消退貨</para>
        /// <para>8:取消授權</para>
        /// </summary>
        public string TradeType { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string AuthIdResp { get; set; }
        public string CardNumber { get; set; }
        public string CardHash { get; set; }
        public string BankNo { get; set; }
        public string CoBranded { get; set; }
        public string CoBrandCardEventCode { get; set; }
        public string CoBrandCardStartDate { get; set; }
        public string CoBrandCardEndDate { get; set; }
        public string PaymentType { get; set; }
        public string RedeemPt { get; set; }
        public string RedeemAmt { get; set; }
        public string PostRedeemPt { get; set; }
    }
}
