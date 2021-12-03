using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    public class WebAPIInput_DecryptCTBCHtml: WebAPIInput_PaymentBase
    {
        
        public string Lidm { get; set; }

        public string PageText { get; set; }
    }
}
