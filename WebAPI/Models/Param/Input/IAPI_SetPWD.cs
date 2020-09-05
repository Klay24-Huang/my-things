using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 設定密碼，供重置密碼後使用
    /// </summary>
    public class IAPI_SetPWD
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