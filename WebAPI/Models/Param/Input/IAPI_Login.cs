using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 登入輸入
    /// </summary>
    public class IAPI_Login
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string PWD { set; get; }
        /// <summary>
        /// DeviceID
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// 語系
        /// <para>0:正體中文</para>
        /// <para>1:簡體中文</para>
        /// <para>2:英文</para>
        /// <para>3:日文</para>
        /// <para>4:韓文</para>
        /// </summary>
        public int Language { set; get; }
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
        //public string PushREGID { set; get; }

    }
}