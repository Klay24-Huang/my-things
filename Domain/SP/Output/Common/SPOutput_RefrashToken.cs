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
        /// 會員流水編號
        /// </summary>
        public string MEMRFNBR { get; set; }
    }
}