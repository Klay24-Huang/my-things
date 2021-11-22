using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 更新 Token
    /// </summary>
    public class WebAPIInput_RefreshToken
    {
        /// <summary>
        /// 取得令牌
        /// </summary>
        public string access_token { get; set; } 

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; } 

    }
}
