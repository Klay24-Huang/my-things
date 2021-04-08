using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.PolygonList
{
    public class SpInput_GetAlliRentStation
    {
        public Int64 LogID { get; set; }
        public double lat { get; set; } = 0;
        public double lng { get; set; } = 0;
        public double radius { get; set; } = 0;
        public string CarTypes { get; set; } = "";
        public string Seats { get; set; } = "";
        public DateTime? SD { get; set; }
        public DateTime? ED { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
