using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 取得公鑰
    /// </summary>
    public class WebAPIInput_GetPublicKey
    {
        /// <summary>
        /// 公鑰類型 (0:取得有效舊公鑰;1:取得新公鑰)
        /// </summary>
        public int type { get; set; } = 0;

    }
}
