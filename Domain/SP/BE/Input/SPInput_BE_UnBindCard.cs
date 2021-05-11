using Domain.SP.Input;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_UnBindCard : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 信用卡密鑰
        /// </summary>
        public string CardToken { get; set; }
    }
}