namespace Domain.TB
{
    public class OrderDetailData
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 營運商
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 車輛圖片
        /// </summary>
        public string CarTypePic { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 座椅數
        /// </summary>
        public int Seat { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string CarBrend { get; set; }

        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 據點名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 評分
        /// </summary>
        public float OperatorScore { get; set; }

        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjName { get; set; }

        /// <summary>
        /// 車輛租金
        /// </summary>
        public int pure_price { get; set; }

        /// <summary>
        /// 月租抵扣
        /// </summary>
        public float MonthlyHours { get; set; }

        /// <summary>
        /// 折抵時數(汽車)
        /// </summary>
        public float GiftPoint { get; set; }

        /// <summary>
        /// 折抵時數(機車)
        /// </summary>
        public float GiftMotorPoint { get; set; }

        /// <summary>
        /// 里程費
        /// </summary>
        public int mileage_price { get; set; }

        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { get; set; }

        /// <summary>
        /// etag費用
        /// </summary>
        public int Etag { get; set; }

        /// <summary>
        /// 逾時費
        /// </summary>
        public int fine_price { get; set; }

        /// <summary>
        /// 代收停車費
        /// </summary>
        public int parkingFee { get; set; }

        /// <summary>
        /// 轉乘優惠折抵
        /// </summary>
        public int TransDiscount { get; set; }

        /// <summary>
        /// 總金額
        /// </summary>
        public int final_price { get; set; }

        /// <summary>
        /// 發票類型
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 捐贈協會名稱
        /// </summary>
        public string NPOBAN_Name { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string invoiceCode { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public string invoice_date { get; set; }

        /// <summary>
        /// 發票金額
        /// </summary>
        public int invoice_price { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { get; set; }
    }
}