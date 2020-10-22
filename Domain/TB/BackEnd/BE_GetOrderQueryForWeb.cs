using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetOrderQueryForWeb
    {
        public string OrderNum { set; get; }
        public string IDNO { set; get; }
        public string CarNo { set; get; }
        public int car_mgt_status { set; get; }
        //public int booking_status { set; get; }
        public int cancel_status { set; get; }
        public string StationName { set; get; }
    }
}
