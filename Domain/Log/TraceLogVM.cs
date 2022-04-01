namespace Domain.Log
{
    public class TraceLogVM
    {
        /// <summary>
        /// 程式版本
        /// </summary>
        public string CodeVersion { get; set; } = "x";

        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; } = 0;

        /// <summary>
        /// APIID
        /// </summary>
        public int ApiId { get; set; } = 0;

        /// <summary>
        /// APIName
        /// </summary>
        public string ApiNm { get; set; } = "x";

        /// <summary>
        /// 
        /// </summary>
        public string ApiMsg { get; set; } = "x";

        /// <summary>
        /// 
        /// </summary>
        public string FlowStep { get; set; } = "x";

        /// <summary>
        /// 
        /// </summary>
        public eumTraceType TraceType { get; set; } = eumTraceType.none;
    }

    public enum eumTraceType
    {
        none,
        fun,
        exception,
        followErr,
        logicErr,
        mark
    }
}
