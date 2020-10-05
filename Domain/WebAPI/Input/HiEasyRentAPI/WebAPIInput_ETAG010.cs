using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// ETAG查詢by合約
    /// </summary>
    public class WebAPIInput_ETAG010
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
        /// iRent訂單編號，含H
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 還車時間，格式yyyyMMddHHmmss
        /// </summary>
        public string RNTDATETIME { set; get; }
    }
}
