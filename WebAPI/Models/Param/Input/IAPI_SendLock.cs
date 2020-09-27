using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SendLock
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// <para>0:解鎖</para>
        /// <para>1:上鎖</para>
        /// </summary>
        public int Lock { set; get; }
    }
}