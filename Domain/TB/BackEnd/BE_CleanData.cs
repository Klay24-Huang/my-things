using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CleanData
    {
        public int OrderNum { set; get; }
        public string UserID { set; get; }
        public int outsideClean { set; get; }
        public int insideClean { set; get; }
        public int rescue { set; get; }
        public int dispatch { set; get; }
        public int Anydispatch { set; get; }
        public int Maintenance { set; get; }
        public int OrderStatus { set; get; }
        public string remark { set; get; }
        public string incarPic { set; get; }
        public string incarPicStr { set; get; }
        public string incarPicType { set; get; }
        public string outcarPic { set; get; }
        public string outcarPicStr { set; get; }
        public string outcarPicType { set; get; }
        public DateTime BookingStart { set; get; }
        public DateTime BookingEnd { set; get; }
        public DateTime lastCleanTime { set; get; }
        public string lastRentTimes { set; get; }
        public string CarNo { set; get; }
        public string lend_place { set; get; }
        public DateTime start_time { set; get; }
        public DateTime stop_time { set; get; }
        public string MKTime { set; get; }
    }
}
