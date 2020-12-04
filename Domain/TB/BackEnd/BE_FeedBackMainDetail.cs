using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_FeedBackMainDetail : BE_FeedBackMain
    {
        public string order_number { set; get; }
        public string CarNo { set; get; }
        public string lend_place { set; get; }
        public string return_place { set; get; }
        public string StationID { set; get; }
        public string nowStationID { set; get; }
        public string StationName { set; get; }
    }
}
