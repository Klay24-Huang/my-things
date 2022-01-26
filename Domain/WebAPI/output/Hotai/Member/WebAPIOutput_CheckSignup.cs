using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    ///  註冊檢查
    /// </summary>
    public class WebAPIOutput_CheckSignup
    {
        /// <summary>
        /// 是否註冊
        /// </summary>
        public bool isSignup { get; set; }

        /// <summary>
        /// 會員狀態(1:半會員(僅有帳密) 2:yoxi會員(缺生日、身分證) 3:會員)
        /// </summary>
        public string status { get; set; }
    }
}
