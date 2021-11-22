using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 修改密碼
    /// </summary>
    public class WebAPIInput_UpdatePassword
    {
        /// <summary>
        /// 原始密碼
        /// </summary>
        public string oldPassword { get; set; }

        /// <summary>
        /// 新密碼
        /// </summary>
        public string newPassword { get; set; }

        /// <summary>
        /// 確認新密碼
        /// </summary>
        public string confirmNewPassword { get; set; }

    }
}
