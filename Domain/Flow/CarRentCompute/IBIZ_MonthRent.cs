using Domain.TB;
using System;
using System.Collections.Generic;

namespace Domain.Flow.CarRentCompute
{
    public class IBIZ_MonthRent
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 intOrderNO { get; set; }

        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { get; set; }

        /// <summary>
        /// 機車基消
        /// </summary>
        public double MotoBasePrice { get; set; }

        /// <summary>
        /// 單日計費最大分鐘數
        /// </summary>
        public double MotoDayMaxMins { get; set; }

        /// <summary>
        /// 每分鐘多少-機車平日
        /// </summary>
        public double MinuteOfPrice { set; get; }

        /// <summary>
        /// 每分鐘多少-機車假日
        /// </summary>
        public float MinuteOfPriceH { get; set; }

        /// <summary>
        /// 是否逾時
        /// </summary>
        public bool hasFine { get; set; }

        /// <summary>
        /// 實際取車時間
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { get; set; }

        /// <summary>
        /// 實際還車時間
        /// </summary>
        public DateTime FED { get; set; }

        /// <summary>
        /// 機車基本分鐘數
        /// </summary>
        public int MotoBaseMins { get; set; }

        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; }

        /// <summary>
        /// 要折抵的點數
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// 平日每小時-汽車
        /// </summary>
        public int PRICE { set; get; }

        /// <summary>
        /// 假日每小時-汽車
        /// </summary>
        public int PRICE_H { set; get; }

        /// <summary>
        /// 汽車基本分鐘數
        /// </summary>
        public int carBaseMins { get; set; }

        /// <summary>
        /// 前n分鐘0元
        /// </summary>
        public double FirstFreeMins { get; set; }

        /// <summary>
        /// 取消所有月租
        /// </summary>
        public bool CancelMonthRent { get; set; } = false;

        /// <summary>
        /// 月租Id(可多筆)
        /// </summary>
        public string MonIds { get; set; }

        /// <summary>
        /// 每日上限金額      // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
        /// </summary>
        public int MaxPrice { get; set; }

        /// <summary>
        /// 標籤優惠分鐘數
        /// </summary>
        public int GiveMinute { get; set; }

        /// <summary>
        /// 虛擬月租
        /// </summary>
        public List<MonthlyRentData> VisMons { get; set; }
    }
}