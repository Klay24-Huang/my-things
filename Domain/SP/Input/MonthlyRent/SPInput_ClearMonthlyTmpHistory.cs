﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.MonthlyRent
{
   public  class SPInput_ClearMonthlyTmpHistory:SPInput_Base
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
    }
}
