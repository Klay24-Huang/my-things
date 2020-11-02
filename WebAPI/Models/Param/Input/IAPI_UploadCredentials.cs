using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 上傳證件照
    /// </summary>
    public class IAPI_UploadCredentials
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
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
        /// <summary>
        /// 證件照類型
        /// <para>1:身份證正面</para>
        /// <para>2:身份證反面</para>
        /// <para>3:汽車駕照正面</para>
        /// <para>4:汽車駕照反面</para>
        /// <para>5:機車駕證正面</para>
        /// <para>6:機車駕證反面</para>
        /// <para>7:自拍照</para>
        /// <para>8:法定代理人</para>
        /// <para>9:其他（如台大專案）</para>
        /// <para>10:企業用戶</para>
        /// <para>11:簽名檔</para>
        /// </summary>
        public Int16? CredentialType { set; get; }

        /// <summary>
        /// 證件照（Base64)
        /// </summary>
        public string CredentialFile { set; get; }
    }
}