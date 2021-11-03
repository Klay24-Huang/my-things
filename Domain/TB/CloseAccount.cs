using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class CloseAccount
    {
        public string OrderNo { set; get; }
        public string MerchantTradeNo { set; get; }
        public int MerchantMemberID { set; get; }
        public DateTime MKTime { set; get; }
    }
}
