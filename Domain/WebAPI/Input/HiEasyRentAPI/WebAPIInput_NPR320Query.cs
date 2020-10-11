using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// 邀請碼
    /// </summary>
   public class WebAPIInput_NPR320Query
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 簽章
        /// </summary>
        public string sig { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 兌換碼
        /// </summary>
        public string COUPONNO { set; get; }
    }
}
