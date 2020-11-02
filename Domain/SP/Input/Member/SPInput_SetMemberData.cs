using System;

namespace Domain.SP.Input.Member
{
    /// <summary>
    /// 更新會員資料
    /// </summary>
    public class SPInput_SetMemberData
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
        /// 連絡電話(住家)
        /// </summary>
        public string MEMHTEL { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { get; set; }
    }
}