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

        /// <summary>
        /// 付費方式：0:信用卡;1:和雲錢包;2:line pay;3:街口支付
        /// </summary>
        public int PayMode { get; set; }
    }


    public class SPInput_UpdateOrderAuthList : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 authSeq { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int AuthFlg { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string AuthCode { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string AuthMessage { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public int Reward { set; get; }
    }
}
