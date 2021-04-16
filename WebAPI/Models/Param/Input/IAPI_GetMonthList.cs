using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetMonthList
    {
        /// <summary>
        /// 是否為機車
        /// <para>0汽車</para>
        /// <para>1機車</para>        
        /// </summary>
        /// <mark>全部月租列表必要參數</mark>
        public int IsMoto { get; set; }
        /// <summary>
        /// 回傳模式
        /// <para>0有使用中月租為2,沒有則為1</para>
        /// <para>1所有月租</para>
        /// <para>2我的方案</para>
        /// </summary>
        public int ReMode { get; set; } = 0;
    }
}