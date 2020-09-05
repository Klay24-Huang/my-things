using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Common
{
    /// <summary>
    /// usp_CheckToken input
    /// </summary>
    public class SPInput_CheckToken
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 裝置代碼
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// APP版號
        /// </summary>
        public string APPVersion { set; get; }
        /// <summary>
        /// 何種APP
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16 APP { set; get; }
        /// <summary>
        /// 此api呼叫的log id 對應TB_APILOG PK
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
