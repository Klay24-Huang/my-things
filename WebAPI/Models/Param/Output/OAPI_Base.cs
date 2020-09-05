using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 基本輸出
    /// </summary>
    public class OAPI_Base
    {
        /// <summary>
        /// 是否成功
        /// <para>1:成功</para>
        /// <para>0:失敗</para>
        /// </summary>
        public int Result { set; get; }
        /// <summary>
        /// 錯誤碼
        /// </summary>
        public string ErrorCode { set; get; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { set; get; }
        /// <summary>
        /// 是否需重新登入
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int NeedRelogin { set; get; }
        /// <summary>
        /// 是否需至商店更新
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int NeedUpgrade { set; get; }
        /// <summary>
        /// 是否需驗證EMail
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int NeedVerifyEmail { set; get; }

    }
}