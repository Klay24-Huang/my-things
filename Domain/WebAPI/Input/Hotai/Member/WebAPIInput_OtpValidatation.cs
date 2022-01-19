using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 驗證會員資訊，取得 OTP 編號
    /// </summary>
    public class WebAPIInput_OtpValidatation
    {
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 驗證類別 (1:身分證字號 2:Email)
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 要驗證的值
        /// </summary>
        public string value { get; set; }

    }
}
