using Domain.WebAPI.output.Hotai.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class IFN_HotaiAddCard: IFN_HotaiPaymenyBase
    {
        public string IDNO { get; set; }

        public string RedirectURL { get; set; }
    }
}
