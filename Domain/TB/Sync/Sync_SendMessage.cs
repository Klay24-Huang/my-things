using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Sync
{
    public class Sync_SendMessage
    {
        public Int64 NotificationID { set; get; }
        /// <summary>
        /// 0是全推
        /// </summary>
        public Int16 NType { set; get; }
        public string UserToken { set; get; }
        public DateTime STime { set; get; }
        public string Message { set; get; }
        public string url { set; get; }
    }
}
