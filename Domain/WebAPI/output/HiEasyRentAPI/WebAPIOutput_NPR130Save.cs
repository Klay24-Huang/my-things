using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
   public  class WebAPIOutput_NPR130Save
    {
        public string Message { get; set; }
        public List<WebAPIOutput_NPR130SaveData> Data { get; set; }
        public bool Result { get; set; }
        public string RtnCode { get; set; }
    }
    public class WebAPIOutput_NPR130SaveData
    {
        public string INVNO { get; set; }
        public string INVDATE { get; set; }
        public int INVAMT { get; set; }
    }
}
