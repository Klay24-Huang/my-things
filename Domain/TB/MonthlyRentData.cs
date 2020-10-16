﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 月租資料（含優惠費率）
    /// </summary>
    public class MonthlyRentData
    {
        /// <summary>
        /// 類別
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int Mode {set;get;} 
        /// <summary>
        /// PK
        /// </summary>
        public Int64 MonthlyRentId { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 汽車平日剩餘時數
        /// </summary>
         public float WorkDayHours {set;get;}
        /// <summary>
        /// 汽車假日剩餘時數
        /// </summary>
         public float HolidayHours {set;get;}
        /// <summary>
        /// 機車剩餘總時數
        /// </summary>
         public float MotoTotalHours {set;get;} 
        /// <summary>
        /// 汽車平日優惠費率
        /// </summary>
         public float WorkDayRateForCar {set;get;} 
        /// <summary>
        /// 汽車假日優惠費率
        /// </summary>
         public float HoildayRateForCar {set;get;} 
        /// <summary>
        /// 機車平日優惠費率
        /// </summary>
         public float WorkDayRateForMoto {set;get;} 
        /// <summary>
        /// 機車假日優惠費率
        /// </summary>
         public float HoildayRateForMoto {set;get;} 
        /// <summary>
        /// 開始時間
        /// </summary>
         public DateTime StartDate { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { set; get; }
    }
}