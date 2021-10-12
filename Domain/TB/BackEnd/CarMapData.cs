using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class CarMapData
    {
        public string CARNO { get; set; }
        public string CID { get; set; }
        public string nowStationID { get; set; }
        public string GPSTime { get; set; }
        public string NonResponse { get; set; }
        public string NonResponseOneHour { get; set; }
        public string Available { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AREA { get; set; }
        public string Memo { get; set; }
        public string device3TBA { get; set; }
        public string deviceMBA { get; set; }
        public string device2TBA { get; set; }
        public string isMotor { get; set; }
    }
}
