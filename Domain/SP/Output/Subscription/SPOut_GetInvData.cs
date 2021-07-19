using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetInvData
    {
        /// <summary>
        /// 發票代碼 from TB_Code
        /// </summary>
        public string InvoiceId { get; set; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string InvoiceType { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { get; set; }
        /// <summary>
        /// 載具條碼/手機條碼
        /// </summary>
        public string CARRIERID { get; set; }
        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN { get; set; }
    }
}
