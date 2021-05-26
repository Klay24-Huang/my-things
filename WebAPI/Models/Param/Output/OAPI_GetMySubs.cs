using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMySubs
    {
        public OAPI_GetMySubs_Month Month { get; set; }
    }

    public class OAPI_GetMySubs_Month
    {
        /// <summary>
        /// 月租專案代碼
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 月租總期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短天期天數
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 月租專案名稱
        /// </summary>
        public string MonProjNM { get; set; }
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public double CarWDHours { get; set; }
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public double CarHDHours { get; set; }
        /// <summary>
        /// 機車不分平假日分鐘數
        /// </summary>
        public int MotoTotalMins { get; set; }  //20210525 ADD BY ADAM REASON.改為INT
        /// <summary>
        /// 汽車平日優惠價
        /// </summary>
        public double WDRateForCar { get; set; }
        /// <summary>
        /// 汽車假日優惠價
        /// </summary>
        public double HDRateForCar { get; set; }
        /// <summary>
        /// 機車平日優惠價
        /// </summary>
        public double WDRateForMoto { get; set; }
        /// <summary>
        /// 機車假日優惠價
        /// </summary>
        public double HDRateForMoto { get; set; }
        /// <summary>
        /// 起日
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 迄日
        /// </summary>
        public string EndDate { get; set; }
        public string MonthStartDate { get; set; }
        public string MonthEndDate { get; set; }
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
    }

    public class OAPI_GetMySubs_Code
    {
        /// <summary>
        /// 代碼編號
        /// </summary>
        public Int64 CodeId { get; set; }
        /// <summary>
        /// 代碼名稱
        /// </summary>
        public string CodeNm { get; set; }
        //public int Sort { get; set; }
        //public string CodeGroup { get; set; }
        /// <summary>
        /// 是否預設選取
        /// </summary>
        public int IsDef { get; set; }
    }
}