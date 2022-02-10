using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    /// <summary>
    /// 同意新款會員權益及隱私條款
    /// </summary>
    public class WebAPIInput_UpdateBenefitsAndPrivacyVersion
    {
        /// <summary>
        /// 會員權益版本號
        /// </summary>
        public string memberBenefitsVersion { get; set; }

        /// <summary>
        /// 隱私條款版本號
        /// </summary>
        public string privacyPolicyVersion { get; set; }

    }
}
