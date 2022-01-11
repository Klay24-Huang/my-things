using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HiEasyRetry:IAPI_BE_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 類型
        /// <para>1:060</para>
        /// <para>2:125</para>
        /// <para>3:130</para>
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// DEBUG用 強制指定
        /// </summary>
        public int retryMode { set; get; } = 0;
    }
}