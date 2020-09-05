using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 重發簡訊
    /// </summary>
    public class IAPI_SendSMS:IAPI_Base
    {
        /// <summary>
        /// 模式
        /// <para>0:驗證手機</para>
        /// <para>1:忘記密碼</para>
        /// <para>2:一次性開門</para>
        /// </summary>
        public Int16? Mode { set; get; }
    }
}