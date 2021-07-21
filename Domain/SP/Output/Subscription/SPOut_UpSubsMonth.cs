using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_UpSubsMonth : SPOutput_Base
    {
        public int xError { get; set; }
        public int MonthlyRentId { get; set; }
        /// <summary>
        /// 升轉時的期數
        /// </summary>
        public int UPDPeriod { get; set; }
        /// <summary>
        /// 原本的起日
        /// </summary>
        public DateTime OriSDATE { get; set; }
    }
}
