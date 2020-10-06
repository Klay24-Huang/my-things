using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_DonePayRent : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// auth token
        /// </summary>
        public string Token { set; get; }
    }
}
