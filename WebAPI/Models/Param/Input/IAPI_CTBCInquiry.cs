using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CTBCInquiry
    {
        /// <summary>
        /// 請款查詢起日
        /// </summary>
        public string QueryBgn { set; get; }

        /// <summary>
        /// 請款查詢迄日
        /// </summary>
        public string QueryEnd { set; get; }

    }
}