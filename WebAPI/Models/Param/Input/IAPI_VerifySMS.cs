using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 驗證簡訊驗證碼
    /// </summary>
    public class IAPI_VerifySMS
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { set; get; }
        /// <summary>
        /// 驗證模式
        /// <para>0:註冊</para>
        /// <para>1:重設密碼</para>
        /// <para>2:一次性開門</para>
        /// </summary>
        public Int16? Mode { set; get; }
        /// <summary>
        /// 機碼
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// APP類型
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16? APP { set; get; }
        /// <summary>
        /// APP版號
        /// </summary>
        public string APPVersion { set; get; }
    }
}