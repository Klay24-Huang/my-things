using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Member
{
    public class SPInput_InsUnBindLog
    {
        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNO { set; get; }

        /// 解綁訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 信用卡密鑰
        /// </summary>
        public string CardToken { set; get; }
        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
