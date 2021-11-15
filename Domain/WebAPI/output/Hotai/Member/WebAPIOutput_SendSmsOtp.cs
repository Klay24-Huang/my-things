using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    public class WebAPIOutput_SendSmsOtp : WebAPIOutput_Base
    {
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string otpCode { get; set; }
    }
}
