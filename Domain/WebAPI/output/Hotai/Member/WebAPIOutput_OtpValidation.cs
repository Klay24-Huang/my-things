using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 驗證會員資訊，取得 OTP 編號
    /// </summary>
    public class WebAPIOutput_OtpValidation : WebAPIOutput_Base
    {
        /// <summary>
        /// OTP 編號(GUID)
        /// </summary>
        public string otpId { get; set; } 
    }
}
