
namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetPayInfo:SPInput_Base
    {
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; } = "";

        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 輸入來源(1:APP;2:Web)
        /// 1:驗證Token/2:不驗證Token
        /// </summary>
        public short InputSource { get; set; } = 2;

    }
}
