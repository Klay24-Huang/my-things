using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 註冊時設定密碼
    /// </summary>
    public class SPInput_Register_Step2
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 裝置ID
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string PWD { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
        /// <summary>
        /// 短租會員流水號
        /// </summary>
        public Int64 MEMRFNBR { set; get; }
    }
}
