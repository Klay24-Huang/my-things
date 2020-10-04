using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetWalletInfo:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        public string Token { set; get; }
    }
}
