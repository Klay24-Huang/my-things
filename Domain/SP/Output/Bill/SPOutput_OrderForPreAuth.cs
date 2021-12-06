using Domain.SP.Output;
using System;

namespace Domain.SP.Output.Bill
{
    /// <summary>
    /// 訂單資訊(For計算預授權金額用)
    /// </summary>
    public class SPOutput_OrderForPreAuth : SPOutput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; }


        /// <summary>
        /// 專案代號
        /// </summary>
        public string ProjID { get; set; }

        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { get; set; }

        /// <summary>
        /// 平日專案價格
        /// </summary>
        public int PRICE { get; set; }

        /// <summary>
        /// 假日專案價格
        /// </summary>
        public int PRICE_H { get; set; }

        /// <summary>
        /// 假日售價
        /// </summary>
        public int WeekdayPrice { get; set; }

        /// <summary>
        /// 平日售價
        /// </summary>
        public int HoildayPrice { get; set; }


        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarTypeGroupCode { get; set; }

        /// <summary>
        /// 是否加購安心服務
        /// </summary>
        public int Insurance { get; set; }

        /// <summary>
        /// 每小時安心服務價格
        /// </summary>
        public int InsurancePerHours { get; set; }

        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { get; set; }

        /// <summary>
        /// 預授權金額
        /// </summary>
        public int PreAuthAmt { get; set; } = 0;

        /// <summary>
        /// 延長次數
        /// </summary>
        public int ExtendTimes { get; set; }

        /// <summary>
        /// 延長開始時間
        /// </summary>
        public DateTime ExtendStartTime { get; set; }

        /// <summary>
        /// 延長結束時間
        /// </summary>
        public DateTime ExtendStopTime { get; set; }

        /// <summary>
        /// 所屬站點
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 是否要預授權 (預留訂閱制不走預授權)
        /// </summary>
        public short DoPreAuth { get; set; } = 1;

    }
}