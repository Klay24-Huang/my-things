using System;

namespace Domain.SP.Input.Register
{
    public class SPInput_ReSendSMS
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 機碼
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 模式
        /// <para>0:註冊</para>
        /// <para>1:忘記密碼</para>
        /// <para>2:一次性開門</para>
        /// <para>3:更換手機</para>
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { get; set; }
    }
}