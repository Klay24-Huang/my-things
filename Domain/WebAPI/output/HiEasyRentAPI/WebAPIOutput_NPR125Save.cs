using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
   public  class WebAPIOutput_NPR125Save
    {
        public string Message { get; set; }
        public List<WebAPIOutput_NPR125SaveData> Data { get; set; }
        public bool Result { get; set; }
        public string RtnCode { get; set; }
    }

    public class WebAPIOutput_NPR125SaveData
    {
        public string CNTRNO { get; set; }
    }
}
