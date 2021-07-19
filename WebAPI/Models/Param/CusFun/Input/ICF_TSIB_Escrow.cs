using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_TSIB_Escrow
    {
        public string IDNO { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }        
        public int Amount { get; set; }
        public string MemberId { get; set; }
    }
}