﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsTradeForClose : SPInput_InsTrade
    {
        public int AutoClose { get; set; }
        public int AuthType { get; set; }
    }

}