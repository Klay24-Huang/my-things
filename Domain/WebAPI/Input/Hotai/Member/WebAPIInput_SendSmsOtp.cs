using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 發送簡訊 OTP
    /// </summary>
    public class WebAPIInput_SendSmsOtp
    {
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string mobilePhone { get; set; } = "";
        
        /// <summary>
        /// OTP 編號 (當 useType 是 2 時此參數為必填)
        /// </summary>
        public string otpId { get; set; } = "";

        /// <summary>
        /// 簡訊類型 (1-註冊;2-忘記密碼;3-修改帳號;4-忘記密碼-半會員)
        /// </summary>
        public int useType { get; set; }
    }
}
