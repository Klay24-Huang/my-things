namespace Domain.SP.Output.Member
{
    public class MemberMedalList
    {
        /// <summary>
        /// 徽章代碼
        /// </summary>
        public string MileStone { get; set; }

        /// <summary>
        /// 徽章名稱
        /// </summary>
        public string MileStoneName { get; set; }

        /// <summary>
        /// 門檻指標
        /// </summary>
        public int Norm { get; set; }

        /// <summary>
        /// 目前進度
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// APP顯示的描述
        /// </summary>
        public string Describe { get; set; }
    }
}