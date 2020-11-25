using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Common
{
    /// <summary>
    /// 更新token
    /// </summary>
    public class SPInput_RefrashToken
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// Refrash Token
        /// </summary>
        public string RefrashToken { set; get; }
        /// <summary>
        /// DeviceID
        /// </summary>
        public string DeviceID { set; get; }

        /// <summary>
        /// APP類型
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16? APP { set; get; }
        /// <summary>
        /// APP版號
        /// </summary>
        public string APPVersion { set; get; }
        /// <summary>
        /// token有效時間
        /// </summary>
        public int Rxpires_in { set; get; }
        /// <summary>
        /// Refrash Token 有效時間
        /// </summary>
        public int Refrash_Rxpires_in { set; get; }
        /// <summary>
        /// 此api呼叫的log id 對應TB_APILOG PK
        /// </summary>
        public Int64 LogID { set; get; }
        /// <summary>
        /// 推播註冊流水號
        /// </summary>
        public int PushREGID { set; get; }
    }
}
