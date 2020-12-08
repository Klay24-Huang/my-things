using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Sync.Input
{
    public class SPInput_SYNC_UpdNotification
    {
        public Int64 NotificationID { set; get; }
        public DateTime PushTime { set; get; }
        public Int16 isSend { set; get; }
    }
}
