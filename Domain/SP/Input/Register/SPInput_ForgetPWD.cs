using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
   public  class SPInput_ForgetPWD
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }

        /// 機碼
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { set; get; }
        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { set; get; }

    
    }
}
