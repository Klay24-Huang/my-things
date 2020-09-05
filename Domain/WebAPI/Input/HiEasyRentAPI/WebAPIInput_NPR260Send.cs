using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR260Send
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
        /// 手機號碼
        /// </summary>
        public string TARGET { set; get; }
        /// <summary>
        /// 簡訊內容 中文70個字1則 英文160個字1則 最大可塞500個字
        /// </summary>
        public string MESSAGE { set; get; }
        /// <summary>
        /// 備註(彈性使用)100個字
        /// </summary>
        public string RENO { set; get; }
    }
}
