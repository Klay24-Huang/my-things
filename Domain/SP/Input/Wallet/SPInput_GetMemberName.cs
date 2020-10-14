namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetMemberName : SPInput_Base
    {
        /// <summary>
        /// 登入者身分證
        /// </summary>
        public string LoginIDNO { get; set; }

        /// <summary>
        /// 轉贈對象身分證
        /// </summary>
        public string DonateIDNO { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { set; get; }
    }
}