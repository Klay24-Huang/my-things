using Domain.WebAPI.output.Taishin.ResultData;
using System.Collections.Generic;

namespace Domain.WebAPI.output.Taishin
{
    public class GetCreditCardListResponseParams
    {
        //   GetCreditCardResponseDataParam
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public List<GetCreditCardResultData> ResultData { get; set; }
    }
}
