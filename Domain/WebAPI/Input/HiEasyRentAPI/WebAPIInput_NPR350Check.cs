using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// NPR350 合約狀態查詢
    /// </summary>
    public class WebAPIInput_NPR350Check
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
        /// iRent OrderNo
        /// </summary>
        public string IRENTORDNO { set; get; }
    }
}
