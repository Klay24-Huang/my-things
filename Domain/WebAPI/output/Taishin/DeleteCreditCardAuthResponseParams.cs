using Domain.WebAPI.output.Taishin.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
   public class DeleteCreditCardAuthResponseParams
    {
        public string ResultCode { set; get; }
        public string ResultMessage { set; get; }
        public DeleteCreditCardAuthResultData ResultData { set; get; }
    }
    
}
