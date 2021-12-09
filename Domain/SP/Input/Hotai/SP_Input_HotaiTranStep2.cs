using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_HotaiTranStep2 : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reqjsonpwd { get; set; }
        public string PrgName { get; set; }
        public string PrgUser { get; set; }
    }
}
