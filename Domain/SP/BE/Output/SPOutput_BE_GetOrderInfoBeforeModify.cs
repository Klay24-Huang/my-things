using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_GetOrderInfoBeforeModify : SPOutput_Base
    {
        public Int16 hasModify   {set;get;}
        public string ModifyTime  {set;get;}
        public string ModifyUserID{set;get;}
        public string LastStartTime { set; get; }
        public string LastStopTime { set; get; }
        public int LastEndMile { set; get; }
    }
}
