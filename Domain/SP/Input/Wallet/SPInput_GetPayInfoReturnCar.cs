using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetPayInfoReturnCar
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 付款金額
        /// </summary>
        public int PaymentAmount { get; set; }
    }
}
