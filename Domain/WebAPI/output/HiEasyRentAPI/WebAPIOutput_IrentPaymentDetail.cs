using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_IrentPaymentDetail
    {
        public bool Result { set; get; }//
        public string RtnCode { set; get; }
        public string Message { set; get; }
        public WebAPIOutput_NPR390QueryData[] Data { set; get; }
    }
}
