using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetCancelOrderList
    {
        /// <summary>
        /// 目前頁數
        /// </summary>
        public int? NowPage { set; get; }
       
    }
}