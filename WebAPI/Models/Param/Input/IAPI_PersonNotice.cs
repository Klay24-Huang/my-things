using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 個人訊息
    /// </summary>
    public class IAPI_PersonNotice
    {
        /// <summary>
        /// <para>0:一般訊息</para>
        /// <para>1:系統訊息</para>
        /// </summary>
        public int? type { set; get; }
    }
}