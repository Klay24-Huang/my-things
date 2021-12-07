using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    public class WebAPIInput_CTBCBase
    {

        /// <summary>
        /// 中信後台的交易識別碼
        /// </summary>
        public string XID { get; set; }

        /// <summary>
        /// 交易授權碼
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 授權交易之代碼
        /// </summary>
        public string AuthRRPID { get; set; }

        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }

        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; }

        /// <summary>
        /// 原交易銷貨金額
        /// </summary>
        public int PurchAmt { get; set; } = 0;

        /// <summary>
        /// 交易幣別代碼，新台幣為 901。
        /// </summary>
        public string Currency { get; set; } = "901";

        /// <summary>
        /// (optional)為幣值指數，新台幣為 0。
        /// </summary>
        public int Exponent { get; set; } = 0;

    }
}
