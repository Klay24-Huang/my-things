using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_DirectCardAuth
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public DirectCardAuthRequestParams OriRequestParams { get; set; }
        public DirectCardAuthResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
}
