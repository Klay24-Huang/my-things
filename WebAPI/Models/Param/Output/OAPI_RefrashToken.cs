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
        /// 強制更新 1=強更，0=否
        /// </summary>
        public int MandatoryUPD { get; set; }
    }
}