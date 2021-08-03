﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class MonCardParam
    {
        /// <summary>
        /// 方案代碼-key
        /// </summary>
        public string MonProjID { get; set; } = "";
        /// <summary>
        /// 方案名稱
        /// </summary>
        public string MonProjNM { get; set; } = "";
        /// <summary>
        /// 總期數-key
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短期總天數,非短期則為0-key
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 方案價格
        /// </summary>
        public int PeriodPrice { get; set; }
        /// <summary>
        /// 是否為機車0否1是
        /// </summary>
        public int IsMoto { get; set; }
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public double CarWDHours { get; set; }
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public double CarHDHours { get; set; }
        /// <summary>
        /// 機車不分平假日時數
        /// </summary>
        public int MotoTotalMins { get; set; }
        /// <summary>
        /// 汽車平日優惠費率
        /// </summary>
        public double WDRateForCar { get; set; }
        /// <summary>
        /// 汽車假日優惠費率
        /// </summary>
        public double HDRateForCar { get; set; }
        /// <summary>
        /// 機車平日優惠費率
        /// </summary>
        public double WDRateForMoto { get; set; }
        /// <summary>
        /// 機車假日優惠費率
        /// </summary>
        public double HDRateForMoto { get; set; }
        /// <summary>
        /// 是否為優惠方案
        /// <para>1是</para>
        /// <para>0否</para>
        /// </summary>
        public int IsDiscount { get; set; }
        /// <summary>
        /// 當期是否有繳費
        /// </summary>
        public int IsPay { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM
        /// </summary>
        public int IsMix { get; set; }
        /// <summary>
        /// 信用卡支付金額 20210716 ADD BY ADAM REASON.把信用卡授權金額跟
        /// </summary>
        public int PayPrice { get; set; }
    }

    public class MonCardParam_My: MonCardParam
    {
        /// <summary>
        /// 下期是否已付款
        /// </summary>
        public int NxtPay { get; set; }
        /// <summary>
        /// 訂閱制月租起日
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 訂閱制月租迄日
        /// </summary>
        public string EndDate { get; set; }
    }

}