using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 註冊檢查
    /// </summary>
    public class WebAPIInput_CheckSignup
    {
        /// <summary>
        /// 帳號(手機號碼)
        /// </summary>
        public string account { get; set; }

    }
}
