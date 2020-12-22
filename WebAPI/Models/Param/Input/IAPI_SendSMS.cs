using System;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 重發簡訊
    /// </summary>
    public class IAPI_SendSMS : IAPI_Base
    {
        /// <summary>
        /// 模式
        /// <para>0:註冊</para>
        /// <para>1:忘記密碼</para>
        /// <para>2:一次性開門</para>
        /// <para>3:更換手機</para>
        /// </summary>
        public Int16? Mode { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
    }
}