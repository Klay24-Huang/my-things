using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// 折抵時數轉贈確認查詢API輸入
    /// </summary>
    public class WebAPIInput_NPR370Check
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
        /// 來源身份證
        /// </summary>
        public string SourceId { set; get; }
        /// <summary>
        /// 目的手機號碼
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 轉贈分鐘數
        /// </summary>
        public int TransMins { set; get; }
    }
}
