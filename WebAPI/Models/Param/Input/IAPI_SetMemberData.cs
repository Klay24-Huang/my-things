using System;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 修改會員資料
    /// </summary>
    public class IAPI_SetMemberData
    {
        /// <summary>
        /// 帳號(身份證)
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 機碼
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// APP類型
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16? APP { get; set; }

        /// <summary>
        /// APP版號
        /// </summary>
        public string APPVersion { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME { get; set; }

        /// <summary>
        /// 生日
        /// <para>格式：yyyy-MM-dd</para>
        /// </summary>
        public string MEMBIRTH { get; set; }

        /// <summary>
        /// 行政區ID
        /// <para>由API AreaList之AreaID帶入</para>
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 會員住址
        /// </summary>
        public string MEMADDR { get; set; }

        /// <summary>
        /// 會員email
        /// </summary>
        public string MEMEMAIL { get; set; }

        /// <summary>
        /// 電子簽名（Base64)
        /// </summary>
        public string Signture { get; set; }

        /// <summary>
        /// 聯絡電話(手機)
        /// </summary>
        public string MEMTEL { get; set; }

        /// <summary>
        /// 公司電話
        /// </summary>
        public string MEMCOMTEL { get; set; }

        /// <summary>
        /// 緊急連絡人
        /// </summary>
        public string MEMCONTRACT { get; set; }

        /// <summary>
        /// 緊急連絡人電話
        /// </summary>
        public string MEMCONTEL { get; set; }

        /// <summary>
        /// 活動及優惠訊息通知 (Y:是 N:否)
        /// </summary>
        public string MEMMSG { get; set; }
    }
}