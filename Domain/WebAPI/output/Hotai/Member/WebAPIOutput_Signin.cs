using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 登入
    /// </summary>
    public class WebAPIOutput_Signin : WebAPIOutput_Base
    {
        /// <summary>
        /// 取得令牌
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 令牌類型
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }


        /// <summary>
        /// 會員狀態(1:半會員(僅有帳密) 2:yoxi會員(缺生日、身分證) 3:會員)
        /// </summary>
        public string memberState { get; set; }

        /// <summary>
        /// Email 是否已驗證
        /// </summary>
        public bool isCheckEmail { get; set; }
    }
}
