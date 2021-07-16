using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
    public class SPOut_GetMonthGroup
    {
        public string MonSetID { get; set; }
        public string MonProjID { get; set; }
        public string MonProjNM { get; set; }
        public int MonProPeriod { get; set; }
        public int PeriodPrice { get; set; }
        public int ShortDays { get; set; }
        public string MonProDisc { get; set; }
        public int IsMoto { get; set; }
        public int MonLvl { get; set; }
        public int MonType { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double CarTotalHours { get; set; }
        public double MotoWDMins { get; set; }
        public double MotoHDMins { get; set; }
        public double MotoTotalMins { get; set; }
        public string SDATE { get; set; }
        public string EDATE { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        /// <summary>
        /// 是否為優惠方案0(否),1(是)
        /// </summary>
        public int IsDiscount { get; set; }
        public int IsOrder { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM REASON.
        /// </summary>
        public int IsMix { get; set; }
        /// <summary>
        /// 可以使用到時麼時候
        /// </summary>
        public DateTime UseUntil { get; set; }
        /// <summary>
        /// 信用卡授權金額 20210716 
        /// </summary>
        public int PayPrice { get; set; }
    }
}
