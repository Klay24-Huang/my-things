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
        /// 使否儲存查詢紀錄:0(不儲存), 1(儲存)
        /// </summary>
        public int IsSave { get; set; }
        /// <summary>
        /// 身份證號
        /// </summary>
        public string IDNO { set; get; }
    }
}