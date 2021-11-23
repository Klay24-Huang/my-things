using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Member
{
    /// <summary>
    /// 取得個人資料
    /// </summary>
    public class WebAPIOutput_GetMemberProfile
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 電子郵件
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 生日(yyyy-MM-dd)
        /// </summary>
        public DateTime birthday { get; set; }

        /// <summary>
        /// 性別(M:男;F:女)
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 身分證字號
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string county { get; set; }

        /// <summary>
        /// 鄉鎮市區
        /// </summary>
        public string township { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
    }
}
