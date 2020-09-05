using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class BindRequestParams
    {
        public string OrderNo { get; set; }
        public string MemberId { get; set; }
        public string ResultUrl { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public string PaymentType { get; set; }
    }
}
