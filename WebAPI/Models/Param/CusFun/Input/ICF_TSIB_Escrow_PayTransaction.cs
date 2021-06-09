using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_TSIB_Escrow_PayTransaction
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public Int64 OrderNo { get; set; }
        public string Token { get; set; }
        public string AccountId { get; set; }
        public int Amount { get; set; }
    }
}