using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Login
{
    /// <summary>
    /// 登入，執行usp_MemberLogin傳入參數
    /// </summary>
    public class SPInput_MemberLogin
    {
        /// <summary>
        /// 帳號
        /// </summary>
       public string  MEMIDNO       { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
       public string  PWD           { set; get; }
        /// <summary>
        /// 裝置代碼
        /// </summary>
        public string  DeviceID      { set; get; }
        /// <summary>
        /// APP版號
        /// </summary>
        public string  APPVersion    { set; get; }
        /// <summary>
        /// 何種APP
        /// <para>0:Android</para>
        /// <para>1:iOS</para>
        /// </summary>
        public Int16  APP { set; get; }
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
        /// 推播註冊流水號 20201118 ADD BY ADAM REASON.增加推播註冊流水號傳入
        /// </summary>
        public int PushREGID { set; get; }
    }
}
