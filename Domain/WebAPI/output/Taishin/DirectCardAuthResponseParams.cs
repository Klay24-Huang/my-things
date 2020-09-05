using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
   public class DirectCardAuthResponseParams
    {
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public DirectCardAuthResultData ResultData { get; set; }
    }
}
