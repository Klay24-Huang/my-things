using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_TSIB_Escrow_PayTransaction
    {
        public string IDNO { get; set; }
        public Int64 OrderNo { get; set; }
        public string MemberID { get; set; }
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public string EcStatus { get; set; }
        public string PRGID { get; set; }
    }
}