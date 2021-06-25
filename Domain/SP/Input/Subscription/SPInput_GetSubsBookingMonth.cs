using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_GetSubsBookingMonth
    {
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 此筆訂單是否已完成履保(排程專用)
        /// </summary>
        /// <mark>取出對應訂單編號知單筆資料時,務必設定為-1</mark>
        public int EscrowStatus { get; set; } = -1;//值為-1時
    }
}
