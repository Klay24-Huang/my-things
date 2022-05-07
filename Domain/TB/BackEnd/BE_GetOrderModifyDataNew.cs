namespace Domain.TB.BackEnd
{
    public class BE_GetOrderModifyDataNew
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 使用者姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string PRONAME { set; get; }
        /// <summary>
        /// 專案類型
        /// </summary>
        public int PROJTYPE { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string ED { set; get; }

        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string FS { set; get; }
        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string FE { set; get; }
        /// <summary>
        /// 實際逾時時間
        /// </summary>
        public string FT { set; get; }
        /// <summary>
        /// 取車里程
        /// </summary>
        public float SM { set; get; }
        /// <summary>
        /// 還車里程
        /// </summary>
        public float EM { set; get; }
        /// <summary>
        /// 里程費
        /// </summary>
        public int mileage_price { set; get; }
        /// <summary>
        /// 總額
        /// </summary>
        public int final_price { set; get; }
        /// <summary>
        /// 逾時罰金
        /// </summary>
        public int fine_price { set; get; }
        /// <summary>
        /// 純租金
        /// </summary>
        public int pure_price { set; get; }
        /// <summary>
        /// 使用汽車點數
        /// </summary>
        public int gift_point { set; get; }
        /// <summary>
        /// 使用機車點數
        /// </summary>
        public int gift_motor_point { set; get; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int parkingFee { set; get; }
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; }
        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { set; get; }
        /// <summary>
        /// 是否付費
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int trade_status { set; get; }
        /// <summary>
        /// 刷卡單號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// ETag費用
        /// </summary>
        public int eTag { set; get; }
        /// <summary>
        /// 使用會員點數(汽車)
        /// </summary>
        public int CarPoint { set; get; }
        /// <summary>
        /// 使用會員點數(機車)
        /// </summary>
        public int MotorPoint { set; get; }
        /// <summary>
        /// 假日價（汽車）
        /// </summary>
        public int HoildayPrice { set; get; }
        /// <summary>
        /// 假日每分鐘（機車）
        /// </summary>
        public float HoildayPriceByMinutes { set; get; }
        /// <summary>
        /// 平日價（汽車）
        /// </summary>
        public int WeekdayPrice { set; get; }
        /// <summary>
        /// 平日每分鐘（機車）
        /// </summary>
        public float WeekdayPriceByMinutes { set; get; }
        /// <summary>
        /// 台新交易編號
        /// </summary>
        public string ServerOrderNo { set; get; }
        /// <summary>
        /// 平日上限（機車）
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 假日上限（機車）
        /// </summary>
        public int MaxPriceH { set; get; }
        /// <summary>
        /// 已付金額
        /// </summary>
        public int Paid { set; get; }
        /// <summary>
        /// 車輛調度費
        /// </summary>
        public int CarDispatch { set; get; }
        /// <summary>
        /// 調度備註
        /// </summary>
        public string DispatchRemark { set; get; }
        /// <summary>
        /// 清潔費
        /// </summary>
        public int CleanFee { set; get; }
        /// <summary>
        /// 清潔費備註
        /// </summary>
        public string CleanFeeRemark { set; get; }
        /// <summary>
        /// 物品損壞/遣失費
        /// </summary>
        public int DestroyFee { set; get; }
        /// <summary>
        /// 物品損壞/遣失備註
        /// </summary>
        public string DestroyFeeRemark { set; get; }
        /// <summary>
        /// 非配合停車費
        /// </summary>
        public int OtherParkingFee { set; get; }
        /// <summary>
        /// 非配合停車費說明
        /// </summary>
        public string ParkingFeeRemark { set; get; }
        /// <summary>
        /// 拖吊費
        /// </summary>
        public int DraggingFee { set; get; }
        /// <summary>
        /// 拖吊費說明
        /// </summary>
        public string DraggingFeeRemark { set; get; }
        /// <summary>
        /// 其他費用
        /// </summary>
        public int OtherFee { set; get; }
        /// <summary>
        /// 其他費用說明
        /// </summary>
        public string OtherFeeRemark { set; get; }
        /// <summary>
        /// 特約停車場費用
        /// </summary>
        public int PARKINGAMT2 { set; get; }
        /// <summary>
        /// 特約停車場說明
        /// </summary>
        public string PARKINGMEMO2 { set; get; }
        /// <summary>
        /// 安心服務每小時價格
        /// </summary>
        public int InsurancePerHours { set; get; }
        /// <summary>
        /// 付款方式(0:信用卡,1:電子錢包)
        /// </summary>
        public string PayMode { get; set; }
        /// <summary>
        /// 優惠贈與時數
        /// </summary>
        public int GiveMin { get; set; }
        /// <summary>
        /// 實際使用贈與時數
        /// </summary>
        public int UseMin { get; set; }
    }
}