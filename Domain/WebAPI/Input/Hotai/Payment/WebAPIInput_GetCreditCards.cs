using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    public class WebAPIInput_GetCreditCards : WebAPIInput_PaymentBase
    {
        public string Bin { get; set; } = "true";
    }
}
