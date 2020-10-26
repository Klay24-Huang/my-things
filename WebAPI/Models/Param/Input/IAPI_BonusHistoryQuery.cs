using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 點數歷程查詢
    /// </summary>
    public class IAPI_BonusHistoryQuery
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 點數流水號
        /// </summary>
        public int SEQNO { set; get; }
    }
}