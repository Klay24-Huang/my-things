using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 取得 Email
    /// </summary>
    public class WebAPIOutput_GetEmail : WebAPIOutput_Base
    {
        /// <summary>
        /// email
        /// </summary>
        public string email { get; set; }
    }
}
