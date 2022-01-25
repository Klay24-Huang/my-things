using System;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SetSubsCreditStatus
    {
        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
        /// <summary>
        /// 呼叫的ApiId
        /// </summary>
        public int APIID { get; set; }
        /// <summary>
        /// 呼叫的Action名稱
        /// </summary>
        public string ActionNM { get; set; }
        /// <summary>
        ///  呼叫台新時傳入Key值, apiIn的json
        /// </summary>
        public string ApiCallKey { get; set; }
        /// <summary>
        /// -呼叫銀行的Ap時傳入Key值, 銀行apiin的json
        /// </summary>
        public string BankApiCallKey { get; set; }
        /// <summary>
        /// 刷卡呼叫成功時銀行的Api回傳, 銀行apiout的json
        /// </summary>
        public string BankApiRe { get; set; }
        /// <summary>
        /// 刷卡結果
        /// <para>0:未發送</para>
        /// <para>1:回傳成功</para>
        /// <para>2:回傳失敗</para>
        /// <para>3:Exception</para>
        /// </summary>
        public int CreditStatus { get; set; }
        /// <summary>
        /// ???
        /// </summary>
        public DateTime? SetNow { get; set; }
        /// <summary>
        /// 刷卡失敗Exception訊息
        /// </summary>
        public string Note { get; set; }
    }
}