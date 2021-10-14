using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPOutput_GetCvsPaymentId : SPOutput_Base
    {
        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string PaymentId { set; get; }

    }
}
