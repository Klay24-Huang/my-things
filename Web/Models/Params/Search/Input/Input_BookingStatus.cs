using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Params.Search.Input
{
    public class Input_BookingStatus
    {
        public string CarNo { get; set; }
        public string OrderNo { get; set; }
        public string IDNO { get; set; }
        public string StationID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Mode { get; set; }
    }
}