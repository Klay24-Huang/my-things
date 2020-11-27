using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.MA.Input
{
    public class SPInpupt_MA_CleanCarStart:SPInput_Base
    {
        public string UserID { set; get; }
        public string CarNo { set; get; }
        public Int64 OrderNum { set; get; }
    }
}
