using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 更新 Token
    /// </summary>
    public class WebAPIOutput_Token :WebAPIOutput_Base
    {
        /// <summary>
        /// 取得令牌
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 令牌類型
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }
    }
}
