using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 檢查會員權益及隱私條款版本
    /// </summary>
    public class WebAPIOutput_BenefitsAndPrivacyVersion : WebAPIOutput_Base
    {
        /// <summary>
        /// 會員權益版本號 (Null 或空白表示無新版資料)
        /// </summary>
        public string memberBenefitsVersion { get; set; }

        /// <summary>
        /// 會員權益 (文字中有\n或\\n，表示需斷行處理)
        /// </summary>
        public string memberBenefits { get; set; }

        /// <summary>
        /// 隱私條款版本號 (Null 或空白表示無新版資料)
        /// </summary>
        public string privacyPolicyVersion { get; set; }

        /// <summary>
        /// 隱私條款 (文字中有\n或\\n，表示需斷行處理)
        /// </summary>
        public string privacyPolicy { get; set; }

    }
}
