using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsTrade:SPInput_Base
    {
        public Int64 OrderNo          {set;get;}
        public string MerchantTradeNo  {set;get;}
        public int CreditType       {set;get;}
        public string MemberID { set; get; }
        public string CardToken { set; get; }


        public int amount           { set; get; }
    }
}
