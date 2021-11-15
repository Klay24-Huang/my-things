using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    ///  使用手機取得會員 OneID
    /// </summary>
    public class WebAPIOutput_GetMobilePhoneToOneID : WebAPIOutput_Base
    {
        /// <summary>
        /// 會員編號(GUID)
        /// </summary>
        public string memberSeq { get; set; }
    }
}
