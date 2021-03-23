namespace Domain.SP.Output.Common
{
    public class SPOutput_RefrashToken : SPOutput_Base
    {
        /// <summary>
        /// 存取token
        /// </summary>
        public string Access_Token { get; set; }

        /// <summary>
        /// refrash token
        /// </summary>
        public string Refrash_Token { get; set; }

        /// <summary>
        /// 強制更新 1=強更，0=否
        /// </summary>
        public int MandatoryUPD { get; set; }
    }
}