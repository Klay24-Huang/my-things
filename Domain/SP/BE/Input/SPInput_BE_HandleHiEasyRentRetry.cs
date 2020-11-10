using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleHiEasyRentRetry:SPInput_Base
    {
        public Int64 OrderNo      {set;get;}
        public Int16 Mode         {set;get;}

        public string UserID { set; get; }
    }
}
