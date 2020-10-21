using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_GetCarMachineAndCheckOrder:SPInput_Base
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
    }
}
