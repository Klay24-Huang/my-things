using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 變更密碼
    /// </summary>
    public class IAPI_ChangePWD
    {
        ///// <summary>
        ///// 帳號
        ///// </summary>
        //public string IDNO { set; get; }
        ///// <summary>
        ///// 機碼
        ///// </summary>
        //public string DeviceID { set; get; }
        ///// <summary>
        ///// APP類型
        ///// <para>0:Android</para>
        ///// <para>1:iOS</para>
        ///// </summary>
        //public Int16? APP { set; get; }
        ///// <summary>
        ///// APP版號
        ///// </summary>
        //public string APPVersion { set; get; }
        /// <summary>
        /// 舊密碼
        /// </summary>
        public string OldPWD { set; get; }
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPWD { set; get; }
    }
}