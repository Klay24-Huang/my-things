using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleHoilday
    {
        public string UserID { get; set; }
        public int QueryYear { set; get; }
        public int QuerySeason { set; get; }
        public List<BE_Hoilday> Hoilday { get; set; }
    }
    public class BE_Hoilday {
        public string HolidayYearMonth { get; set; }
        public string HolidayDate { get; set; }
    }
}