using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Other
{
    public class SPInput_TestPushService_I01 : SPInput_Base
    {
        public string IDNO { get; set; }
        public int PushRegID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
    }
}
