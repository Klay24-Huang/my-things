using System;

namespace Domain.SP.Input.Register
{
    public class SPInput_GetVerifyCode
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 模式
        /// <para>0:註冊</para>
        /// <para>1:忘記密碼</para>
        /// <para>2:一次性開門</para>
        /// <para>3:更換手機</para>
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { set; get; }
    }
}