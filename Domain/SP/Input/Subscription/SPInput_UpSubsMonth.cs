using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_UpSubsMonth
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string UP_MonProjID { get; set; }
        public int UP_MonProPeriod { get; set; }
        public int UP_ShortDays { get; set; }
        public Int64 PayTypeId { get; set; }
        public Int64 InvoTypeId { get; set; }
        public DateTime? SetNow { get; set; }
        public string MerchantTradeNo { get; set; }
        public string TaishinTradeNo { get; set; }
    }
}
