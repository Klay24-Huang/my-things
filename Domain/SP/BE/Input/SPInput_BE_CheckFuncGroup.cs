﻿using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_CheckFuncGroup:SPInput_Base
    {
        public string FuncGroupID { set; get; }
        public string UserID { set; get; }
    }
}
