using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_FeedBackHandle:IAPI_BE_Base
    {
        public Int64 FeedBackID { set; get; }
        /// 處理訊息
        /// </summary>
        public string HandleDescript { set; get; }
    }
}