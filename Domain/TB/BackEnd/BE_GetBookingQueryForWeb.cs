using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetBookingQueryForWeb
    {
        public string OrderNum { set; get; }
        public string IDNO { set; get; }
        public string CarNo { set; get; }
        public int car_mgt_status { set; get; }
        //public int booking_status { set; get; }
        public int cancel_status { set; get; }
        public string StationName { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { set; get; }
    }
}
