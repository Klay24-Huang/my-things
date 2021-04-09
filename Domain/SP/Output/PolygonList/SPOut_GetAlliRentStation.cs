using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.PolygonList
{
    public class SPOut_GetAlliRentStation
    {
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string Tel { get; set; }
        public string ADDR { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Content { get; set; }
        public string IsRent { get; set; }
    }
}
