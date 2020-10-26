using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR271Query
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }
        /// <summary>
        /// 身份證號
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 時數流水號
        /// </summary>
        public int SEQNO { set; get; }
    }
}
