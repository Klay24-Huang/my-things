namespace WebAPI.Models.Param.Output
{
    public class OAPI_OrderDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 合約網址
        /// </summary>
        public string ContactURL { get; set; }

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
        public int CarRentBill { get; set; }

        /// <summary>
        /// 使用時數
        /// </summary>
        public string TotalHours { get; set; }

        /// <summary>
        /// 月租抵扣
        /// </summary>
        public string MonthlyHours { get; set; }

        /// <summary>
        /// 折抵時數(汽車+機車)
        /// </summary>
        public string GiftPoint { get; set; }

        /// <summary>
        ///  計費時數
        /// </summary>
        public string PayHours { get; set; }

        /// <summary>
        /// 里程費
        /// </summary>
        public int MileageBill { get; set; }

        /// <summary>
        /// 安心服務費
        /// </summary>
        public int InsuranceBill { get; set; }

        /// <summary>
        /// etag費用
        /// </summary>
        public int EtagBill { get; set; }

        /// <summary>
        /// 逾時費
        /// </summary>
        public int OverTimeBill { get; set; }

        /// <summary>
        /// 代收停車費
        /// </summary>
        public int ParkingBill { get; set; }

        /// <summary>
        /// 轉乘優惠折抵
        /// </summary>
        public int TransDiscount { get; set; }

        /// <summary>
        /// 總金額
        /// </summary>
        public int TotalBill { get; set; }

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
        /// 捐贈協會名稱
        /// </summary>
        public string NPOBAN_Name { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string Unified_business_no { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 發票金額
        /// </summary>
        public int InvoiceBill { get; set; }

        /// <summary>
        /// 發票網址
        /// </summary>
        public string InvoiceURL { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public float Millage { get; set; }

        /// <summary>
        /// 據點區域
        /// </summary>
        public string CarOfArea { get; set; }

        /// <summary>
        /// 優惠折抵金額
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// 折抵專案名稱
        /// </summary>
        public string DiscountName { get; set; }

        /// <summary>
        /// 營損-車輛調度費 20201212 ADD BY ADAM
        /// </summary>
        public int CtrlBill { get; set; }

        /// <summary>
        /// 營損-清潔費 20201212 ADD BY ADAM
        /// </summary>
        public int ClearBill { get; set; }

        /// <summary>
        /// 營損-物品損壞 20201212 ADD BY ADAM
        /// </summary>
        public int EquipBill { get; set; }

        /// <summary>
        /// 營損-非約定停車費 20201212 ADD BY ADAM
        /// </summary>
        public int ParkingBill2 { get; set; }

        /// <summary>
        /// 營損-拖吊費 20201212 ADD BY ADAM
        /// </summary>
        public int TowingBill { get; set; }

        /// <summary>
        /// 營損-其他費用 20201212 ADD BY ADAM
        /// </summary>
        public int OtherBill { get; set; }

        /// <summary>
        /// 使用訂金
        /// </summary>
        public int UseOrderPrice { get; set; }
        /// <summary>
        /// 返還=訂金
        /// </summary>
        public int ReturnOrderPrice { get; set; }
        /// <summary>
        /// 換電時數 20210517 ADD BY ADAM
        /// </summary>
        public int ChangePoint { get; set; }
        /// <summary>
        /// 換電次數 20210517 ADD BY ADAM
        /// </summary>
        public int ChangeTimes { get; set; }
        /// <summary>
        /// 取車電量 20210517 ADD BY ADAM
        /// </summary>
        public float RSOC_S { get; set; }
        /// <summary>
        /// 還車電量 20210517 ADD BY ADAM
        /// </summary>
        public float RSOC_E { get; set; }
        /// <summary>
        /// 獎勵時數 20210517 ADD BY ADAM
        /// </summary>
        public int RewardPoint { get; set; }
        /// <summary>
        /// 總回饋時數 20210517 ADD BY ADAM
        /// </summary>
        public int TotalRewardPoint { get; set; }

        /// <summary>
        /// 承租人類型 1:主要承租人 2:共同承租人
        /// </summary>
        public int RenterType { get; set; }
    }
}