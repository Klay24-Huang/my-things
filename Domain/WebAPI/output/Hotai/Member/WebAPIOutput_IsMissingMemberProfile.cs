using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 缺少個人資料
    /// </summary>
    public class WebAPIOutput_IsMissingMemberProfile : WebAPIOutput_Base
    {
        /// <summary>
        /// 是否缺少個資
        /// </summary>
        public bool isMissing { get; set; }

        /// <summary>
        /// 是否缺少姓名
        /// </summary>
        public bool missingName { get; set; }

        /// <summary>
        /// 是否缺少email
        /// </summary>
        public bool missingEmail { get; set; }

        /// <summary>
        /// 是否缺少生日
        /// </summary>
        public bool missingBirthday { get; set; }

        /// <summary>
        /// 是否缺少稱謂
        /// </summary>
        public bool missingSex { get; set; }

        /// <summary>
        /// 是否缺少身份證字號
        /// </summary>
        public bool missingId { get; set; }

        /// <summary>
        /// 會員狀態(1:半會員(僅有帳密) 2:yoxi 會員(缺生日、身分證) 3:會員)
        /// </summary>
        public string memberState { get; set; }
    }
}
