namespace Domain.SP.Input.Member
{
    public class SPInput_MemberUnBind : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 功能名稱
        /// </summary>
        public string APIName { get; set; }
    }
}