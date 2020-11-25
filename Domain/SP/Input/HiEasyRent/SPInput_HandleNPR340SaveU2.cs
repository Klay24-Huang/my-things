using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.HiEasyRent
{
    public class SPInput_HandleNPR340SaveU2:SPInput_Base
    {
        public string MerchantTradeNo { set; get; }
        public string ServerTradeNo { set; get; }
        public int isRetry { set; get; }
    }
}
