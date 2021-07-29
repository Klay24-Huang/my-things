using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsCreditStatus
    {
        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 呼叫的ApiId
        /// </summary>
        public int APIID { get; set; }
        /// <summary>
        /// 呼叫的Action名稱
        /// </summary>
        public string ActionNM { get; set; }
        /// <summary>
        /// (Key) 呼叫台新時傳入Key值, apiIn的json
        /// </summary>
        public string ApiCallKey { get; set; }
        /// <summary>
        /// 呼叫銀行的Ap時傳入Key值, 銀行apiin的json
        /// </summary>
        public string BankApiCallKey { get; set; }
        /// <summary>
        /// 刷卡呼叫成功時銀行的Api回傳, 銀行apiout的json
        /// </summary>
        public string BankApiRe { get; set; }
        /// <summary>
        /// 刷卡結果0(未發送),1(回傳成功),2(回傳失敗),3(excetion)
        /// </summary>
        public int CreditStatus { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime MKTime { get; set; }
    }
}
