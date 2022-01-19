using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 登入
    /// </summary>
    public class WebAPIInput_Signin
    {
        /// <summary>
        /// 帳號(手機號碼)
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 密碼(6-12位英數字)
        /// </summary>
        public string password { get; set; }
    }
}
