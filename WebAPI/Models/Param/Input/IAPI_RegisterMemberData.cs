using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 會員註冊基本資料
    /// </summary>
    public class IAPI_RegisterMemberData
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
        /// 姓名
        /// </summary>
        public string MEMCNAME { set; get; }
        /// <summary>
        /// 生日
        /// <para>格式：yyyy-MM-dd</para>
        /// </summary>
        public string MEMBIRTH { set; get; }
        /// <summary>
        /// 行政區ID
        /// <para>由API AreaList之AreaID帶入</para>
        /// </summary>
        public int AreaID { set; get; }
        /// <summary>
        /// 會員住址
        /// </summary>
        public string MEMADDR { set; get; }
        /// <summary>
        /// 會員email
        /// </summary>
        public string MEMEMAIL { set; get; }
        /// <summary>
        /// 電子簽名（Base64)
        /// </summary>
        public string Signture { set; get; }
    }
}