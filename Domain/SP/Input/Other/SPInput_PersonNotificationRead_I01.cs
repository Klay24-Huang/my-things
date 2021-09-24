using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Other
{
    public class SPInput_PersonNotificationRead_I01 : SPInput_Base
    {
        public string IDNO { get; set; }
        public Int64 NotificationID { get; set; }
    }
}
