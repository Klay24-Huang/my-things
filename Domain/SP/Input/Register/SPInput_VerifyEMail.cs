using System;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 驗證EMail
    /// </summary>
    public class SPInput_VerifyEMail
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// EMAIL
        /// </summary>
        public string EMAIL { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
}