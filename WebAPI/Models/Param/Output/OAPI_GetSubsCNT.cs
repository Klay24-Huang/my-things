using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetSubsCNT
    {
        public OAPI_GetSubsCNT_Card NowCard { get; set; }
        public OAPI_GetSubsCNT_Card NxtCard { get; set; }
    }

    public class OAPI_GetSubsCNT_Card
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
        public double MotoTotalMins { get; set; }
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
        /// <summary>
        /// 注意事項
        /// </summary>
        public string MonProDisc { get; set; }
    }

}