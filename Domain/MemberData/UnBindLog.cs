using System;

namespace Domain.MemberData
{
    public class UnBindLog
    {
        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 解綁訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 信用卡密鑰
        /// </summary>
        public string CardToken { set; get; }
    }
}