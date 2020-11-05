using System;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 寫入註冊基本資料
    /// </summary>
    public class SPInput_RegisterMemberData
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 裝置ID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime MEMBIRTH { get; set; }

        /// <summary>
        /// 行政區ID
        /// </summary>
        public int MEMCITY { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string MEMADDR { get; set; }

        /// <summary>
        /// EMAIL
        /// </summary>
        public string MEMEMAIL { get; set; }

        /// <summary>
        /// 簽名檔檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { get; set; }
    }
}