using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    /// <summary>
    /// 退貨
    /// </summary>
    public class WebAPIInput_EC_Refund
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public string CheckSum { get; set; }
      
        public string Random { get; set; }
        public EC_RefundRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }

    }
    public class EC_RefundItem
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string NonRedeem { get; set; }
        public string NonPoint { get; set; }
    }
    public class EC_RefundRequestParams
    {
        public string CardToken { get; set; }
        public List<EC_RefundItem> Item { get; set; }
        public string MerchantTradeDate { get; set; }
        public string MerchantTradeNo { get; set; }
   
        public string MerchantTradeTime { get; set; }
        public string OriMerchantTradeNo { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string ResultUrl { get; set; }
        public string TradeAmount { get; set; }
      
        public string TradeType { get; set; }
       
       
        
       
    }
}
