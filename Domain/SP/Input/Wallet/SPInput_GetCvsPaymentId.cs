﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetCvsPaymentId : SPInput_Base
    {
        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 業者識別代碼
        /// </summary>
        public string CvsIdentifier { set; get; }
    }
}
