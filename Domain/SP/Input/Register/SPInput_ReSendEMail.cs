using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 重發EMail
    /// </summary>
    public class SPInput_ReSendEMail
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
        /// EMAIL
        /// </summary>
        public string EMAIL { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
