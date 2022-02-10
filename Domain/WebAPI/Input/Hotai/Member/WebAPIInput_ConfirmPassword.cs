using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 確認密碼
    /// </summary>
    public class WebAPIInput_ConfirmPassword
    {
        /// <summary>
        /// 密碼
        /// </summary>
        public string password { get; set; }

    }
}
