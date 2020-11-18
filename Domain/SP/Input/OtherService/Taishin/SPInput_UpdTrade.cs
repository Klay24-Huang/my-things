using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_UpdTrade : SPInput_Base
    {
        public Int64 OrderNo { set; get; }
        public string MerchantTradeNo { set; get; }
        public string MerchantMemberID { set; get; }
        public string RetCode         {set;get;}
        public string RetMsg          {set;get;}
        public string TaishinTradeNo  {set;get;}
        public string CardNumber      {set;get;}
        public DateTime process_date    {set;get;}
        public int AUTHAMT         {set;get;}
        public int AuthIdResp      {set;get;}
        public int IsSuccess       {set;get;}
    }
}
