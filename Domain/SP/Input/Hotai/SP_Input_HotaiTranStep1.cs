using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_HotaiTranStep1 : SPInput_Base
    {
     
        public Int64 OrderNo { set; get; }
        public int CreditType { set; get; }
        public string MemberID { set; get; }
        public string CardToken { set; get; }
        public int amount { set; get; }
        public int AutoClose { get; set; }
        public int AuthType { get; set; }
        public string MerchantTradeNoLeft { get; set; }
        public string PrgName { get; set; }
        public string PrgUser { get; set; }

        /// <summary>
        /// 廠商代碼(金流單位提供)
        /// </summary>
        public string MerchantID { get; set; }
    }
}
