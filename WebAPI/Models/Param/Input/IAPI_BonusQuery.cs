using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 點數查詢
    /// </summary>
    public class IAPI_BonusQuery
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
    }
}