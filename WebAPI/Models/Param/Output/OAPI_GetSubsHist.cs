using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetSubsHist
    {
        /// <summary>
        /// 歷史紀錄
        /// </summary>
        public List<OAPI_GetSubsHist_Param> Hists { get; set; }
    }

    public class OAPI_GetSubsHist_Param
    {
        /// <summary>
        /// 專案代號key
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 專案總期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短天期
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string MonProjNM { get; set; }
        /// <summary>
        /// 每期價格
        /// </summary>
        public int PeriodPrice { get; set; }
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
        public int MotoTotalMins { get; set; }
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
        /// 付款日期
        /// </summary>
        public string PayDate { get; set; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMoto { get; set; }
        /// <summary>
        /// 單期月租起日
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 單期月租迄日
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 付款期數
        /// </summary>
        public int PerNo { get; set; }
        /// <summary>
        /// 月租Id
        /// </summary>
        public Int64? MonthlyRentId { get; set; }
        /// <summary>
        /// 發票設定
        /// </summary>
        public string InvType { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string unified_business_no { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string invoiceCode { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string invoice_date { get; set; }
        /// <summary>
        /// 發票價格
        /// </summary>
        public int? invoice_price { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 add by adam
        /// </summary>
        public int IsMix { get; set; }
    }
}