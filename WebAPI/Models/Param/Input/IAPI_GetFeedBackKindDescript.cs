using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetFeedBackKindDescript
    {
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
    }
}