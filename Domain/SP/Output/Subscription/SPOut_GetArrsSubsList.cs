using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetArrsSubsList
    {
        public List<SPOut_GetArrsSubsList_Date> DateRange { get; set; }
        public List<SPOut_GetArrsSubsList_Card> Arrs { get; set; }
    }

    public class SPOut_GetArrsSubsList_Date
    {
        public Int64 SubsId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SPOut_GetArrsSubsList_Card
    {
        public Int64 SubsId { get; set; }
        public string MonProjNM { get; set; }
        public string ProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public int rw { get; set; }
        public int PeriodPayPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //20210617 ADD BY ADAM REASON.增加車型對照圖
        public string CarTypePic { get; set; }
        //20210717 ADD BY ADAM REASON.MonthlyRentId改由sp取得
        public int MonthlyRentId { get; set; }
        public int IsMoto { get; set; }
    }

}
