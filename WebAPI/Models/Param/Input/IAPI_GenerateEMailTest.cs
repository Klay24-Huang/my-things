using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 產生測試用的EMail驗證碼
    /// </summary>
    public class IAPI_GenerateEMailTest
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// EMail
        /// </summary>
        public string EMail { set; get; }
    }
}