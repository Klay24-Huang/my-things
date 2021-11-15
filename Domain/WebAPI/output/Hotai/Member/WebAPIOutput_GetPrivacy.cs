using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 取得會員權益及隱私條款
    /// </summary>
    public class WebAPIOutput_GetPrivacy : WebAPIOutput_Base
    {     
        /// <summary>
        /// 會員權益 (文字中有\n或\\n，表示需斷行處理)
        /// </summary>
        public string memberBenefits { get; set; }

        /// <summary>
        /// 隱私條款 (文字中有\n或\\n，表示需斷行處理)
        /// </summary>
        public string privacyPolicy { get; set; }

    }
}
