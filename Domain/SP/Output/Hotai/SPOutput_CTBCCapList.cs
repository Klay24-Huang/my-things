using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Hotai
{
    public class SPOutput_CTBCCapList 
    {

        /// <summary>
        /// 特店訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 中信訂單編號
        /// </summary>
        public string XID { get; set; }

        /// <summary>
        /// 關帳金額
        /// </summary>
        public int CloseAmout { get; set; }

        /// <summary>
        /// 授權金額
        /// </summary>
        public int AuthAmt { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// SSL授權交易代碼
        /// </summary>
        public string Authrrpid { get; set; }

        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; }

        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }


    }
}
