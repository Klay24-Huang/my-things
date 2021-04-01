using Domain.SP.Input;
using System;

namespace Domain.Sync.Input
{
    public class SPInput_SYNC_UPDEventMessage : SPInput_Base
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int64 AlertID { get; set; }

        /// <summary>
        /// 發送信箱
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 是否發送
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// <para>2:失敗</para>
        /// </summary>
        public int HasSend { get; set; }

        /// <summary>
        /// 寄送時間
        /// </summary>
        public DateTime SendTime { get; set; }
    }
}