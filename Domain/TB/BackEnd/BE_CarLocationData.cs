using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarLocationData
    {
        public Int64 order_number { get; set; }
        public string IDNO { get; set; }
        public string CarNo { get; set; }
        public string PRONAME { get; set; }
        public string ProjID { get; set; }
        public string lend_place { get; set; }
        public DateTime final_start_time { get; set; }
        public DateTime final_stop_time { get; set; }
        public string CID { get; set; }
        public string start_Lat { get; set; }
        public string start_Lng { get; set; }
        public string stop_Lat { get; set; }
        public string stop_Lng { get; set; }
    }
}
