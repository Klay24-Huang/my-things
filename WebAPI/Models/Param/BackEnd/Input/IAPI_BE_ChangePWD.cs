using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_ChangePWD
    {
        ///// <summary>
        ///// 帳號
        ///// </summary>
        public string Account { set; get; }

        /// <summary>
        /// 舊密碼
        /// </summary>
        public string OldPWD { set; get; }
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPWD { set; get; }
    }
}