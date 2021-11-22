using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    public class WebAPIInput_AddCard: WebAPIInput_PaymentBase
    {
        public string RedirectURL { get; set; }
    }
}
