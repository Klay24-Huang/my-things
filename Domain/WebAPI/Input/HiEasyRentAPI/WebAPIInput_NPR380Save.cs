using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR380Save
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
        /// 會員身分證號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 贈送時數
        /// </summary>
        public string POINT { set; get; }
        /// <summary>
        /// 綁定合約編號
        /// </summary>
        public string CNTRNO { set; get; }
    }
}
