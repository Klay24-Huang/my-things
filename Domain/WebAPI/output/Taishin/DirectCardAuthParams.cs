using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class DirectCardAuthParams
    {
        public string CardData { get; set; }
        public string ResultUrl { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
    }
}
