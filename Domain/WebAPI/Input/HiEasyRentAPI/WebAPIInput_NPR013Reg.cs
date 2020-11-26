using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR013Reg
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
        /// 身分證字號
        /// </summary>
        public string MEMIDNO { get; set; }
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string MEMCNAME { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string MEMPWD { get; set; }
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string MEMCEIL { get; set; }
    }
}
