using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 修改帳號
    /// </summary>
    public class WebAPIInput_UpdateAccount
    {
        /// <summary>
        /// 帳號(手機號碼) (數字 12碼以內)
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 驗證碼(格式：文數字 6碼)
        /// </summary>
        public string otpCode { get; set; }

    }
}
