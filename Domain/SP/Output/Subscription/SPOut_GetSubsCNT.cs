using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsCNT
    {
        public SPOut_GetSubsCNT_NowCard NowCard { get; set; }
        public SPOut_GetSubsCNT_NxtCard NxtCard { get; set; }
    }

    public class SPOut_GetSubsCNT_NowCard
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MonProDisc { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM
        /// </summary>
        public int IsMix { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        /// <summary>
        /// 下期續訂總期數
        /// </summary>
        public int NxtMonProPeriod { get; set; }
        /// <summary>
        /// 是否已升級
        /// </summary>
        public int IsUpd { get; set; }
        /// <summary>
        /// 是否自動續訂
        /// </summary>
        public int SubsNxt { get; set; }
        /// <summary>
        /// 是否變更下期合約
        /// </summary>
        public int IsChange { get; set; }
        /// <summary>
        /// 當期是否有繳費
        /// </summary>
        public int IsPay { get; set; }
        /// <summary>
        /// 是否為機車   //20210527 ADD BY ADAM
        /// </summary>
        public int IsMoto { get; set; }
    }

    public class SPOut_GetSubsCNT_NxtCard
    {
        /// <summary>
        /// 是否已變更下期合約
        /// </summary>
        public int IsChange { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public string MonProDisc { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM
        /// </summary>
        public int IsMix { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        /// <summary>
        /// 下期續訂總期數
        /// </summary>
        public int NxtMonProPeriod { get; set; }
        /// <summary>
        /// 是否已升級
        /// </summary>
        public int IsUpd { get; set; }
        /// <summary>
        /// 是否自動續訂
        /// </summary>
        public int SubsNxt { get; set; }
        /// <summary>
        /// 當期是否有繳費
        /// </summary>
        public int IsPay { get; set; }
        /// <summary>
        /// 是否為機車   //20210527 ADD BY ADAM
        /// </summary>
        public int IsMoto { get; set; }
    }

}
