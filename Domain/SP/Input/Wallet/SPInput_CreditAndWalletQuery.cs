namespace Domain.SP.Input.Wallet
{
    public class SPInput_CreditAndWalletQuery : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }
    }
}