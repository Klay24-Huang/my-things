using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Car
{
    public class SPOutBase_GetStationCarType: SPOutput_Base
    {
        public int IsFavStation { get; set; }
        public string IsRent { get; set; }
    }
}
