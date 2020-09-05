using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class BindResponseParams
    {
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public BindResultData ResultData { get; set; }
    }
}
