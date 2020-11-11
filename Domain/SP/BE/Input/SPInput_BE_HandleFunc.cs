using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleFunc:SPInput_Base
    {
        public int FuncGroupID{set;get;}
        public string Power      {set;get;}
        public string Mode       {set;get;}
        public string UserID { set; get; }

    }
}
