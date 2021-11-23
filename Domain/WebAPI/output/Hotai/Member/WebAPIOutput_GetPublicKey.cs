using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 取得公鑰
    /// </summary>
    public class WebAPIOutput_GetPublicKey
    {
        /// <summary>
        /// 序號
        /// </summary>
        public int seq { get; set; }

        /// <summary>
        /// 公鑰
        /// </summary>
        public string key { get; set; }
    }  
}
