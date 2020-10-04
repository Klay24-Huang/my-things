using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WallHistory
    {
        /// <summary>
        /// 頁數，一頁200筆
        /// </summary>
        public int page { set; get; }
    }
}