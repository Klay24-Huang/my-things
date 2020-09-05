using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 點擊確認信呼叫
    /// </summary>
    public class IAPI_VerifyEMail
    {
        /// <summary>
        /// 加密後的Code
        /// </summary>
        public string VerifyCode { set; get; }
    }
}