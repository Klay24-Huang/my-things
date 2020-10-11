using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_OpenDoor
    {
        /// <summary>
        /// 可申請的期限
        /// </summary>
        public string DeadLine { set; get; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { set; get; }
    }
}