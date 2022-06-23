using Domain.TB.Hotai;
using System.Collections.Generic;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 遊戲項目查詢
    /// </summary>
    public class OAPI_GetGameItem
    {
        /// <summary>
        /// 遊戲外部連結
        /// </summary>
        public string GameSrc { get; set; }
        /// <summary>
        /// 參數(遊戲Token經AES128加密處理)
        /// </summary>
        public string P { get; set; }
    }
}