using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 發送 Email OTP
    /// </summary>
    public class WebAPIInput_SendEmailOtp
    {
        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; } 

    }
}
