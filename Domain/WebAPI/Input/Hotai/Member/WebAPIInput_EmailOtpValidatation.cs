using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// Email OTP 驗證
    /// </summary>
    public class WebAPIInput_EmailOtpValidatation
    {
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string otpCode { get; set; }
    }
}
