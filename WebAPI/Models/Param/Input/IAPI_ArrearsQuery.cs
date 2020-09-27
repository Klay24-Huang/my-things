using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 欠費查詢
    /// </summary>
    public class IAPI_ArrearsQuery
    {
        /// <summary>
        /// 身份證號
        /// </summary>
        public string IDNO { set; get; }
    }
}