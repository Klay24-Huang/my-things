using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    public class WebAPIInput_RelayPost_WalletWithdraw
    {
        public string BaseUrl { get; set; }
        public string ApiUrl { get; set; }
        public string UtcTimeStamp { get; set; }
        public string SignCode { get; set; }
        public string RequestData { get; set; }
    }
}
