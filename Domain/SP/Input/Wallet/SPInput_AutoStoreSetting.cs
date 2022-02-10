namespace Domain.SP.Input.Wallet
{
    public class SPInput_AutoStoreSetting : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 是否同意自動儲值
        /// <para>0:不同意</para>
        /// <para>1:同意</para>
        /// </summary>
        public int AutoStored { get; set; }

        /// <summary>
        /// API Name
        /// </summary>
        public string ApiName { get; set; }
    }
}