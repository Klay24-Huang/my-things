using System;

namespace Domain.SP.Input.Member
{
    public class SPInput_SetMemberCMK : SPInput_Base
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 同意書版本類型
        /// </summary>
        public string VerType { get; set; }

        /// <summary>
        /// 同意書版本號
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 流水號
        /// </summary>
        public int SeqNo { get; set; }

        /// <summary>
        /// 同意來源管道
        /// <para>I:IRENT</para>
        /// <para>W:官網</para>
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 同意時間
        /// </summary>
        public DateTime AgreeDate { get; set; }

        /// <summary>
        /// 電話通知狀態
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string TEL { get; set; }

        /// <summary>
        /// 簡訊通知狀態
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string SMS { get; set; }

        /// <summary>
        /// EMAIL通知
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string EMAIL { get; set; }

        /// <summary>
        /// 郵寄通知
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string POST { get; set; }

        /// <summary>
        /// 來源程式
        /// </summary>
        public string APIName { get; set; }
    }
}