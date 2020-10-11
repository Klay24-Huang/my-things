using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_MixAuth
    {
        /// <summary>
        /// 付款模式
        /// <para>0:租金</para>
        /// <para>1:罰金/補繳</para>
        /// </summary>
        public int PayType { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 罰金或補繳代碼
        /// </summary>
        public string CNTRNO { set; get; }

    }
}