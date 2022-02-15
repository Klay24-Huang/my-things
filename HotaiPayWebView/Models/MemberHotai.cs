using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class MemberHotai
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 和泰ONEID
        /// </summary>
        public string OneID { get; set; }
        /// <summary>
        /// 和泰會員TOKEN
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 和泰會員REFRESHTOKEN
        /// </summary>
        public string RefreshToken { get; set;}
        /// <summary>
        /// 個人資料補填狀態(0:無補填個人資料; 1:有補填個人資料)
        /// </summary>
        public int ProfileStatus { get; set; }
        /// <summary>
        /// 和泰會員狀態(會員中心回覆)
        /// </summary>
        public int MbrStatus { get; set; }
        /// <summary>
        /// R:註冊; L:登入
        /// </summary>
        public string DataType { get; set; }

    }
}