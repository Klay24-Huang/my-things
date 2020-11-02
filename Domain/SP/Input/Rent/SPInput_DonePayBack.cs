﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_DonePayBack: SPInput_Base
    {
        /// <summary>
        /// 欠款查詢主表ID
        /// </summary>
        public int NPR330Save_ID { get; set; }
        public string IDNO { get; set; }
        public string Token { get; set; }
    }
}
