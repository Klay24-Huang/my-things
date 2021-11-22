using Domain.WebAPI.Input.Hotai.Payment.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    public class WebAPIInput_FastAddCard: WebAPIInput_PaymentBase
    {

        public string RedirectURL { get; set; }

        //身分證字號
        public string IDNO { get; set; }
        //生日(YYYYMMDD)
        public string Birthday { get; set; }

    }
}
