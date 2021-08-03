using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_IrentPaymentDetail
    {
        public string user_id { get; set; } = "HLC";
        public string sig { get; set; } = "C2CC1897317CEC6B1378FCCE7FA632142D7399CB";
        public int MODE { get; set; }
        public string SPSD { set; get; }
        public string SPED { set; get; }
        public string SPSD2 { set; get; }
        public string SPED2 { set; get; }
        public string SPSD3 { get; set; }
        public string SPED3 { get; set; }
        public string MEMACCOUNT { set; get; }
    }
}
