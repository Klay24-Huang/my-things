using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMySubs
    {
        public OAPI_GetMySubs_Month Month { get; set; }
        public List<OAPI_GetMySubs_Code> PayTypes { get; set; }
        public List<OAPI_GetMySubs_Code> InvoTypes { get; set; }

    }

    public class OAPI_GetMySubs_Month
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double WorkDayHours { get; set; }
        public double HolidayHours { get; set; }
        public double MotoTotalHours { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 是否自動續訂
        /// </summary>
        public int SubsNxt { get; set; }
        /// <summary>
        /// 是否變更下期合約
        /// </summary>
        public int IsChange { get; set; }
    }

    public class OAPI_GetMySubs_Code
    {
        public Int64 CodeId { get; set; }
        public string CodeNm { get; set; }
        //public int Sort { get; set; }
        //public string CodeGroup { get; set; }
        /// <summary>
        /// 是否預設選取
        /// </summary>
        public int IsDef { get; set; }
    }
}