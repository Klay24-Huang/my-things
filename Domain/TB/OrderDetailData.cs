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
        /// 折抵時數(汽車)
        /// </summary>
        public float GiftPoint { get; set; }

        /// <summary>
        /// 折抵時數(機車)
        /// </summary>
        public float GiftMotorPoint { get; set; }

        /// <summary>
        /// 月租抵扣
        /// </summary>
        public float MonthlyHours { get; set; }

        /// <summary>
        /// 專案類型：0:同站;3:路邊;4:機車
        /// </summary>
        public int ProjType { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { get; set; }

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
        /// 總金額
        /// </summary>
        public int final_price { get; set; }

        /// <summary>
        /// 轉乘優惠折抵
        /// </summary>
        public int TransDiscount { get; set; }

        /// <summary>
        /// 代收停車費
        /// </summary>
        public int parkingFee { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public string invoice_date { get; set; }

        /// <summary>
        /// 發票金額
        /// </summary>
        public int invoice_price { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string invoiceCode { get; set; }

        /// <summary>
        /// 發票類型
        /// <para>1:愛心碼</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 手機條碼/自然人憑證
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string Unified_business_no { get; set; }

        /// <summary>
        /// 捐贈協會名稱
        /// </summary>
        public string NPOBAN_Name { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public float Millage { get; set; }

        /// <summary>
        /// 據點區域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 取車里程
        /// </summary>
        public float Start_mile { get; set; }

        /// <summary>
        /// 還車里程
        /// </summary>
        public float End_mile { get; set; }

        /// <summary>
        /// 優惠折抵金額
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// 折抵專案名稱
        /// </summary>
        public string DiscountName { get; set; }

        /// <summary>
        /// 車輛調度費
        /// </summary>
        public int CarDispatch { get; set; }

        /// <summary>
        /// 清潔費
        /// </summary>
        public int CleanFee { get; set; }

        /// <summary>
        /// 物品損壞/遣失費
        /// </summary>
        public int DestroyFee { get; set; }

        /// <summary>
        /// 非配合停車費
        /// </summary>
        public int ParkingFee2 { get; set; }

        /// <summary>
        /// 拖吊費
        /// </summary>
        public int DraggingFee { get; set; }

        /// <summary>
        /// 其他費用
        /// </summary>
        public int OtherFee { get; set; }

        /// <summary>
        /// 使用訂金
        /// </summary>
        public int UseOrderPrice { get; set; }
        /// <summary>
        /// 返還=訂金
        /// </summary>
        public int ReturnOrderPrice { get; set; }
        /// <summary>
        /// 剩餘訂金
        /// </summary>
        public int LastOrderPrice { get; set; }
    }
}