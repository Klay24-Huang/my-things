using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_IrentPaymentDetail
    {
        public string user_id { get; set; } //="HLC";
        public string sig { get; set; } //= "B0356BE0C53A9E8E5037EB5EAA1E7AEDC448BB0D";
        public int MODE { get; set; }
        public string SPSD { set; get; }
        public string SPED { set; get; }//
        public string SPSD2 { set; get; }
        public string SPED2 { set; get; }
        public string SPSD3 { get; set; }
        public string SPED3 { get; set; }
        public string MEMACCOUNT { set; get; }
    }
}
