using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_VER_USER_POW
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string VerID { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string VerPwd { set; get; }
    }
}