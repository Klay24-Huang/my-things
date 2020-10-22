using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_CarScheduleTimeLog
    {
        public string CarNo { set; get; }
        public List<BE_OrderInfo> lstOrder { set; get; }
    }
    public class BE_OrderInfo
    {
        public string OrderNum { set; get; }
        public string IDNO { set; get; }

        public string UName { set; get; }
        public string Mobile { set; get; }
        public int car_mgt_status { set; get; }
        public int booking_status { set; get; }
        public int cancel_status { set; get; }
        public string SD { set; get; }
        public string ED { set; get; }
        public string FS { set; get; }
        public string FE { set; get; }
    }
}