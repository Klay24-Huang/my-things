using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetMySubs
    {
        public List<SPOut_GetMySubs_Month> Months { get; set; }
    }

    public class SPOut_GetMySubs_Month
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double WorkDayHours { get; set; }
        public double HolidayHours { get; set; }
        public double MotoTotalHours { get; set; }
        public double WorkDayRateForCar { get; set; }
        public double HoildayRateForCar { get; set; }
        public double WorkDayRateForMoto { get; set; }
        public double HoildayRateForMoto { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        /// <summary>
        /// 下期續訂總期數
        /// </summary>
        public int NxtMonProPeriod { get; set; }
        /// <summary>
        /// 是否為城市車手
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMix { get; set; }
        /// <summary>
        /// 是否已升級
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsUpd { get; set; }
        /// <summary>
        /// 是否自動續
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int SubsNxt { get; set; }
        /// <summary>
        /// 是否變更下期合約
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsChange { get; set; }
        /// <summary>
        /// 當期是否有繳費
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsPay { get; set; }
    }

    public class SPOut_GetMySubs_Code
    {
        public Int64 CodeId { get; set; }
        public string CodeNm { get; set; }
        public int Sort { get; set; }
        public string CodeGroup { get; set; }
        /// <summary>
        /// 是否預設選取
        /// </summary>
        public int IsDef { get; set; }
    }

}
