namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetWalletStoredMoneySet : SPInput_Base
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 身分證
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// 儲值方式(1信用卡,2虛擬帳號,3超商繳費)
        /// </summary>
        public int StoreType { set; get; }
    }
}