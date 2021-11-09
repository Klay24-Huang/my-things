using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetPreCreditAuthJob
    {
        /// <summary>
        /// 取車日的前N天 (預設2天)
        /// </summary>
        public int Nday { get; set; } = 2;
        /// <summary>
        /// 取車時間的前N小時 (預設6小時)
        /// </summary>
        public int NHour { get; set; } = 6;
        /// <summary>
        /// 第一次要授權的時間點(目前預設為取車日前N天的20點)
        /// </summary>
        public int FirstReserveTime { get; set; } = 20;
        /// <summary>
        /// 信用卡即時授權通道數(預設1個)
        /// </summary>
        public int AuthGateCount { get; set; } = 1;
        /// <summary>
        /// 信用卡排程授權通道數(預設1個)
        /// </summary>
        public int ReservationAuthGateCount { get; set; } = 1;
        /// <summary>
        /// 逾時預收租金的天數
        /// </summary>
        public int PrepaidDays { get; set; } = 1;

        //int Nday = 2;
        //int NHour = 6;
        //int FirstReserveTime = 20;
        //int AuthGateCount = 1;
        //int ReservationAuthGateCount = 1;
        //int PrepaidDays = 1;


    }
}