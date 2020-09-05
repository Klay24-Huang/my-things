using Domain.Common;
using Domain.MemberData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 登入後回傳
    /// </summary>
    public class OAPI_Login
    {
        /// <summary>
        /// OAuth需使用，Token type=>Bearer
        /// </summary>
      public  Token Token { set; get; }
        /// <summary>
        /// 使用者基本資料
        /// </summary>
      public  RegisterData UserData { set; get; }

    }
}