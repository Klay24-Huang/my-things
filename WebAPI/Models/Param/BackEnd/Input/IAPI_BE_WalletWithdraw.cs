﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_WalletWithdraw : IAPI_BE_Base
    {
        public string IDNO { get; set; }
        public string cashAmount { get; set; }
    }
}