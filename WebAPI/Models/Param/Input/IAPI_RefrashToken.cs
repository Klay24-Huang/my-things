using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 更新token
    /// </summary>
    public class IAPI_RefrashToken
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// Refrash Token
        /// </summary>
        public string RefrashToken { set; get; }
        /// <summary>
        /// DeviceID
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

        /// <summary>
        /// 推播註冊流水號 等APP改好再更新上去
        /// </summary>
        public int PushREGID { set; get; }
    }
}