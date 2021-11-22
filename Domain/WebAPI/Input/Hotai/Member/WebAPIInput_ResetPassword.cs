using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 重設密碼
    /// </summary>
    public class WebAPIInput_ResetPassword
    {
        /// <summary>
        /// 帳號(手機號碼)
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 確認密碼
        /// </summary>
        public string confirmPassword { get; set; }

        /// <summary>
        /// 驗證碼流水號
        /// </summary>
        public string otpId { get; set; }

    }
}
