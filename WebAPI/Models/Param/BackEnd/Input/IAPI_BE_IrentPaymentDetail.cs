using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_IrentPaymentDetail : IAPI_BE_Base
    {
        public string SPSD { get; set; }
        public string SPED { get; set; }
        public string SPSD2 { get; set; }
        public string SPED2 { get; set; }
        public string MEMACCOUNT { get; set; }
    }
}