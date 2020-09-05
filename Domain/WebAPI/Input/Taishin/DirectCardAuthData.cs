using Domain.WebAPI.output.Taishin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    public class DirectCardAuthData
    {
        /// <summary>
        /// 版號
        /// </summary>
        public string ApiVer { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ApposId { set; get; }
        /// <summary>
        /// 隨機碼
        /// </summary>
        public string Random { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DirectCardAuthParams RequestParams { set; get; }
        /// <summary>
        /// 電文產生時間
        /// </summary>
        public string TimeStamp { set; get; }
        public string TransNo { set; get; }
    }
}
