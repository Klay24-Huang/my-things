using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_Auth
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public AuthOriRequestParams OriRequestParams { get; set; }
        public AuthResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
}
