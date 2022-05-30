using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_GetCarHornFlgByCID : SPInput_Base
    {
        public string CID { get; set; }
        
    }
}
