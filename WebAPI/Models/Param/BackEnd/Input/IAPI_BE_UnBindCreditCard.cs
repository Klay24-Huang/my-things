using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_UnBindCreditCard: IAPI_BE_Base
    {
        /// <summary>
        /// 使用者身份證號
        /// </summary>
        public string IDNO { set; get; }
    }
}