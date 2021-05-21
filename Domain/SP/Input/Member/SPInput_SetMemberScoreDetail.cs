namespace Domain.SP.Input.Member
{
    public class SPInput_SetMemberScoreDetail : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int SEQ { get; set; }

        /// <summary>
        /// 程式代號
        /// </summary>
        public int PRGID { get; set; }
    }
}