using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台車輛行程管理
    /// </summary>
    public class BE_CarScheduleTimeLog
    {
        public string OrderNum { set; get; }
        public string IDNO { set; get; }
        public string CarNo { set; get; }
        public string UName { set; get; }
        public string Mobile { set; get; }
        public int car_mgt_status { set; get; }
        public int booking_status { set; get; }
        public int cancel_status { set; get; }
        public DateTime SD { set; get; }
        public DateTime ED { set; get; }
        public DateTime FS { set; get; }
        public DateTime FE { set; get; }
    }
}
