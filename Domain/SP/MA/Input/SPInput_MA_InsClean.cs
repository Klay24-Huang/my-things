using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.MA.Input
{
   public  class SPInput_MA_InsClean:SPInput_Base
    {
        public string manager { set; get; }
        public string CarNo { set; get; }
        public DateTime SD { set; get; }
        public DateTime ED { set; get; }
        public string SpecCode { set; get; }
    }
}
