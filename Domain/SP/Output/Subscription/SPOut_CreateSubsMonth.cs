﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_CreateSubsMonth: SPOutput_Base
    {
        public int xError { get; set; }
        /// <summary>
        /// 20210714 ADD BY ADAM 
        /// </summary>
        public int MonthlyRentId { get; set; }
    }
}
