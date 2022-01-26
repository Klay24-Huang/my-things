using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 取得後台金鑰有效版本號
    /// </summary>
    public class WebAPIInput_GetValidKeyVersion
    {
        /// <summary>
        /// 平台代號
        /// </summary>
        public string appid { get; set; }

    }
}
