namespace Domain.SP.Input.Member
{
    public class SPInput_GetMemberData : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 是否檢查Token (0:不檢查 1:要檢查)
        /// </summary>
        public int CheckToken { get; set; } = 1;

    }
}