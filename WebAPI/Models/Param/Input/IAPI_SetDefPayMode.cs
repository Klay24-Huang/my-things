using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_SetDefPayMode
    {
        /// <summary>
        /// 預設的支付方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// <para>2:line pay</para>
        /// <para>3:街口支付</para>
        /// </summary>
        public int DefPayMode { set; get; }
    }
}