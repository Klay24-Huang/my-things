﻿using Domain.TB;
using System;

namespace Domain.WebAPI.output.rootAPI
{
    /// <summary>
    /// 機車專案
    /// </summary>
    public class MotorProjectObj
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjName { set; get; }

        /// <summary>
        /// 優惠專案描述
        /// </summary>
        public string ProDesc { get; set; }

        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型代碼(群組代碼)
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車型圖片
        /// </summary>
        public string CarTypePic { set; get; }

        /// <summary>
        /// 業者icon
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 業者評分
        /// </summary>
        public Single OperatorScore { set; get; }

        /// <summary>
        /// 是否可加購安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 加購安心服務每小時費用

        /// </summary>
        public int InsurancePerHour { set; get; }
        /// <summary>
        /// 是否是最低價
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMinimum { set; get; } = 0;
        /// <summary>
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 基本費
        /// </summary>
        public int BasePrice { set; get; }
        /// <summary>
        /// 每分鐘n元
        /// </summary>
        public Single PerMinutesPrice { set; get; }
        /// <summary>
        /// 每日上限
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 站別類型
        /// </summary>
        public string CarOfArea { get; set; }
        /// <summary>
        /// 其他備註
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 剩餘電量
        /// </summary>
        public float Power { set; get; }
        /// <summary>
        /// 剩餘里程
        /// </summary>
        public float RemainingMileage { set; get; }

        #region 訂閱制月租
        /// <summary>
        /// 訂閱制月租Id
        /// </summary>
        public Int64 MonthlyRentId { get; set; } = 0;
        /// <summary>
        /// 機車不分平假日時數
        /// </summary>
        public double MotoTotalMins { get; set; }
        /// <summary>
        /// 機車平日優惠價
        /// </summary>
        public double WDRateForMoto { get; set; }
        /// <summary>
        /// 機車假日優惠價
        /// </summary>
        public double HDRateForMoto { get; set; }
        /// <summary>
        /// 開始日(月租全期)
        /// </summary>
        public string MonthStartDate { get; set; } = "";
        /// <summary>
        /// 結束日(月租全期)
        /// </summary>
        public string MonthEndDate { get; set; } = "";
        #endregion

        /// <summary>
        /// 優惠標籤
        /// </summary>
        public ProjectDiscountLabel DiscountLabel { get; set; } = new ProjectDiscountLabel();

        #region 機車安心服務
        /// <summary>
        /// 機車安心服務基消分鐘數
        /// </summary>
        public int BaseInsuranceMinutes { set; get; }
        /// <summary>
        /// 安心服務起始費用(類似機車基消)
        /// </summary>
        public int BaseMotoRate { set; get; }
        /// <summary>
        /// 計次分鐘單位(幾分鐘算一次錢)
        /// </summary>
        public int InsuranceMotoMin { set; get; }
        /// <summary>
        /// 計次金額(每分鐘單位多少錢)
        /// </summary>
        public int InsuranceMotoRate { set; get; }
        #endregion

        /// <summary>
        /// 企業身分統一編號
        /// </summary>
        public string TaxID { get; set; }
    }
}