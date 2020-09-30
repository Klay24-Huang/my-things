using Domain.WebAPI.Input.Taishin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_DeleteCreditCardAuth
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public WebAPIInput_DeleteCreditCardAuth OriRequestParams { get; set; }
        public DeleteCreditCardAuthResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
}
