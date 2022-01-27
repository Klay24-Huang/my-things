using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_BuyNowAddMonth_Q01
    {
        public string IDNO { get; set; }
        public string MonProjId { get; set; }
        public string UP_MonProjID { get; set; } = "";
        public int MonProPeriod { get; set; }
        public int UP_MonProPeriod { get; set; } = 0;
        public int ShortDays { get; set; } 
        public int UP_ShortDays { get; set; } = 0;

        public Int64 LogID { get; set; }
        /// <summary>
        /// 驗證類型 0:月租購買 1:升轉
        /// </summary>
        public int VerifyType { get; set; } = 0;
    }
}
