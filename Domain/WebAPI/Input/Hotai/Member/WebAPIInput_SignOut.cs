using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 登出
    /// </summary>
    public class WebAPIInput_SignOut
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }
    }
}
