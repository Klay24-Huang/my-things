using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_RelayPost
    {
        public bool IsSuccess { get; set; } 
        public string RtnMessage { get; set; }
        public string ResponseData { get; set; }

    }
}
