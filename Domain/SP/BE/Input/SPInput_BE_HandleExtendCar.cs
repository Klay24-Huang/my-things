﻿using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleExtendCar:SPInput_Base
    {
        public Int64 OrderNo { set; get; }
        public DateTime ExtendTime { set; get; }
        public string UserID  { set; get; }
    }
}