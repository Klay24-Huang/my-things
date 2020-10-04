using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_SetingParkingSpaceByReturn:SPInput_Base
    {
        public string IDNO { set; get; }
        public Int64 OrderNo { set; get; }
        public string ParkingSpace { set; get; }
        public string ParkingSpaceImage { set; get; }
        public string Token { set; get; }
   
    }
}
