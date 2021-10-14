
namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetPayInfo:SPInput_Base
    {
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { get; set; }

    }
}
