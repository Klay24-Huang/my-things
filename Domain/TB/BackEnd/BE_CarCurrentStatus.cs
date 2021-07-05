using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarCurrentStatus
    {
        public string CarNo { get; set; }
        public string CID { get; set; }
        public string nowStationID { get; set; }
        public string GPSTime { get; set; }
        public string NonResponse { get; set; }
        public string NonResponseOneHour { get; set; }
        public string Available { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CarOfArea { get; set; }
        public string Memo { get; set; }

    }
}
