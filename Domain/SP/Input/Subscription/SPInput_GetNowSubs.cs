using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_GetNowSubs
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public DateTime? SD { get; set; }
        public DateTime? ED { get; set; }
        public int IsMoto { get; set; } = -1;

        /// <summary>
        /// 用車行程
        /// 1 = 個人身分，2 = 企業身分
        /// </summary>
        public Int16 CarTrip { get; set; } 
    }
}
