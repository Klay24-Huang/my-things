using System;

namespace Domain.SP.Input.Register
{
    public class SPInput_GetMemberMobile
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// 機碼
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { get; set; }
    }
}