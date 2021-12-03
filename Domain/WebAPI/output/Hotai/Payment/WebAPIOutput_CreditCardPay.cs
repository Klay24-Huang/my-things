using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_CreditCardPay: WebAPOutput_PaymentBase
    {
        public string PageTitle { get; set; }

        public string PageText { get; set; }
       
       public int ProtocolStatusCode { get; set; }

    }
}
