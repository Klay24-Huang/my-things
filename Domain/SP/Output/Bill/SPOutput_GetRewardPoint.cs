using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Bill
{
    public class SPOutput_GetRewardPoint : SPOutput_Base
    {
        /// <summary>
        /// 換電獎勵時數
        /// </summary>
        public int Reward { set; get; }
    }
}
