using Domain.Common;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_RefrashToken
    {
        /// <summary>
        /// OAuth需使用，Token type=>Bearer
        /// </summary>
        public Token Token { set; get; }
    }
}