using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_IrentPaymentDetail
    {
        public string SPSD { set; get; }
        public string SPED { set; get; }
        public string SPSD2 { set; get; }
        public string SPED2 { set; get; }
        public string MEMACCOUNT { set; get; }
    }
}
