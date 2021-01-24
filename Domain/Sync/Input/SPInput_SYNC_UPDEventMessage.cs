using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Sync.Input
{
    public class SPInput_SYNC_UPDEventMessage:SPInput_Base
    {
        public Int64 AlertID { set; get; }
        /// <summary>
        /// 發送信箱
        /// </summary>
        public string Sender { set; get; }
        /// <summary>
        /// 是否發送
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// <para>2:失敗</para>
        /// </summary>
        public int HasSend { set; get; }
        public string UserID { set; get; } = "SYS";
    }
}
