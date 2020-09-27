﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Booking
{
    public class SPOutput_Booking:SPOutput_Base
    {
        /// <summary>
        /// 是否有預約到車
        /// </summary>
        public Int16 haveCar { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
       public Int64 OrderNum { set; get; }
    }
}
