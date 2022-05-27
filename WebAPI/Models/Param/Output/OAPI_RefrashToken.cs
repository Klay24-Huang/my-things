using Domain.Common;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_RefrashToken
    {
        /// <summary>
        /// OAuth需使用，Token type=>Bearer
        /// </summary>
        public Token Token { set; get; }
        /// <summary>
        /// 會員流水號
        /// </summary>
        public string MEMRFNBR { set; get; }
    }
}