﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SetSubsPayInvoDef
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public Int64 PayTypeId { get; set; }
        public Int64 InvoTypeId { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
