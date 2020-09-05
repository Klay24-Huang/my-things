using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 驗證碼確認，呼叫usp_CheckVerifyCode
    /// </summary>
    public class SPInput_CheckVerifyCode
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
        /// 訂單編號
        /// </summary>
        public string OrderNum { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:註冊</para>
        /// <para>1:一次性開門</para>
        /// </summary>
        public Int16 Mode { set; get; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
