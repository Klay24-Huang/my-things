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
        public int IsMoto { get; set; }
        /// <summary>
        /// 月租分類
        /// <para>0無</para>
        /// <para>1月租</para>
        /// <para>2訂閱制</para>
        /// <para>3短租</para>
        /// </summary>
        public int MonType { get; set; }
    }
}