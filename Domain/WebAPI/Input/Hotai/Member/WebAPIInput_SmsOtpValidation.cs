using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 簡訊 OTP 驗證
    /// </summary>
    public class WebAPIInput_SmsOtpValidation
    {
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string mobilePhone { get; set; } = "";

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string otpCode { get; set; } = "";

        /// <summary>
        /// 簡訊類型 (1-註冊;2-忘記密碼;3-修改帳號)
        /// </summary>
        public int useType { get; set; }

    }
}
