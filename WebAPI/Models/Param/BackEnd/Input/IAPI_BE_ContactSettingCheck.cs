using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_ContactSettingCheck
    {
        public string OrderNo { set; get; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}