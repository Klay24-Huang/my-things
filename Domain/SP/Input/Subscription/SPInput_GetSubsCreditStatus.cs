using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_GetSubsCreditStatus
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 呼叫的ApiId(必填)
        /// </summary>
        public int APIID { get; set; }
        /// <summary>
        /// 呼叫的Action名稱(選填)
        /// </summary>
        public string ActionNM { get; set; }
        /// <summary>
        /// 呼叫台新時傳入Key值(必填,唯一值) 
        /// </summary>
        public string ApiCallKey { get; set; }
        /// <summary>
        /// 交易時間
        /// </summary>
        public DateTime? SetNow { get; set; }
    }
}
