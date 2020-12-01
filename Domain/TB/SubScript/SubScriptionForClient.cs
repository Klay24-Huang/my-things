using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.SubScript
{
    /// <summary>
    /// 資料更新列表
    /// </summary>
   public  class SubScriptionForClient
    {
        public DateTime EndDate { get; set; }

        public float HolidayHours { get; set; }

        public string IDNO { get; set; }

        public float MotoTotalHours { get; set; }

        public float NowHolidayHours { get; set; }

        public float NowMotoTotalHours { get; set; }

        public float NowWorkDayHours { get; set; }

        public string ProjID { get; set; }

        public string ProjNM { get; set; }

        public int RateType { get; set; }

        public DateTime StartDate { get; set; }

        public float WorkDayHours { get; set; }

    }
}
