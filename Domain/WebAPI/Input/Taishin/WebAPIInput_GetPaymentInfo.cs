using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
   public  class WebAPIInput_GetPaymentInfo
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public string TransNo { get; set; }
        public IGetPaymentInfoRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
    public class IGetPaymentInfoRequestParams
    {
        public string ServiceTradeNo { get; set; }
    }


    public class WebAPIInput_GetECOrderInfo
    {
        public string ApiVer { get; set; }
        public string ApposId { get; set; }
        public string TransNo { get; set; }
        public IGetECOrderInfoRequestParams RequestParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
    public class IGetECOrderInfoRequestParams
    {
        public string MemberId { get; set; }
        public string MerchantTradeNo { get; set; }
    }
}
