using System;

namespace Domain.SP.Input.Rent
{
    public class SPInput_CalFinalPrice : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }

        /// <summary>
        /// 總價
        /// </summary>
        public int final_price { set; get; } = 0;

        /// <summary>
        /// 車輛租金
        /// </summary>
        public int pure_price { set; get; } = 0;

        /// <summary>
        /// 里程費
        /// </summary>
        public int mileage_price { set; get; } = 0;

        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { set; get; } = 0;

        /// <summary>
        /// 逾時費用
        /// </summary>
        public int fine_price { set; get; } = 0;

        /// <summary>
        /// 使用時數(汽車)
        /// </summary>
        public int gift_point { set; get; } = 0;

        /// <summary>
        /// 使用時數(機車)
        /// </summary>
        public int gift_motor_point { get; set; } = 0;

        /// <summary>
        /// 使用的月租平日時數（含機車）
        /// </summary>
        public double monthly_workday { get; set; } = 0;

        /// <summary>
        /// 使用的月租假日時數
        /// </summary>
        public double monthly_holiday { get; set; } = 0;

        /// <summary>
        /// ETAG費用
        /// </summary>
        public int Etag { set; get; } = 0;

        /// <summary>
        /// 特約停車場停車費
        /// </summary>
        public int parkingFee { set; get; } = 0;
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; } = 0;

        /// <summary>
        /// auth token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 差額
        /// </summary>
        public int DiffAmount { get; set; }

        /// <summary>
        /// API名稱
        /// </summary>
        public string APIName { get; set; }

        /// <summary>
        /// 使用標籤優惠分鐘數
        /// </summary>
        public int UseGiveMinute { get; set; }
    }
}