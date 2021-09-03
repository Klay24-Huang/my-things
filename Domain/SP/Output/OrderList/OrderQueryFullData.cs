using System;

namespace Domain.SP.Output.OrderList
{
    /// <summary>
    /// 訂單完整資料
    /// </summary>
    public class OrderQueryFullData
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        #region 據點相關
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 據點名稱，對應tb裡的Location
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 電話
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 其他說明
        /// </summary>
        public string Content { set; get; }
        #endregion

        #region 營運商相關              
        /// <summary>
        /// 營運商名稱
        /// </summary>
        public string OperatorName { set; get; }
        /// <summary>
        /// 營運商icon
        /// </summary>
        public string OperatorICon { set; get; }
        /// <summary>
        /// 評分
        /// </summary>
        public float Score { set; get; }
        #endregion
        #region 車型相關
        /// <summary>
        /// 廠牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車子位置
        /// </summary>
        public string CarOfArea { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車型圖片
        /// </summary>
        public string CarTypeImg { set; get; }
        /// <summary>
        /// 座椅數
        /// </summary>
        public int Seat { set; get; }
        /// <summary>
        /// 停車格位置
        /// </summary>
        public string parkingSpace { set; get; }

        /// <summary>
        /// 車型簡碼
        /// </summary>
        public string CarTypeGroupCode { get; set; }
        #endregion

        #region 機車相關電力相關
        /// <summary>
        /// 電池平均
        /// </summary>
        public float device3TBA { set; get; }
        /// <summary>
        /// 剩餘里程
        /// </summary>
        public string RemainingMilage { set; get; }
        #endregion

        #region 專案相關
        public string ProjID { get; set; }
        /// <summary>
        /// 專案類型
        /// <para>0:同站</para>
        /// <para>3:路邊</para>
        /// <para>4:機車</para>
        /// </summary>
        public int ProjType { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string PRONAME { set; get; }
        /// <summary>
        /// 平日每小時
        /// </summary>
        public int PRICE { set; get; }
        /// <summary>
        /// 假日每小時
        /// </summary>
        public int PRICE_H { set; get; }
        /// <summary>
        /// 春節訂金
        /// </summary>
        public int OrderPrice { get; set; }
        /// <summary>
        /// 使用訂金
        /// </summary>
        public int UseOrderPrice { get; set; }
        /// <summary>
        /// 剩餘訂金
        /// </summary>
        public int LastOrderPrice { get; set; }
        /// <summary>
        /// 前n分鐘0元
        /// </summary>
        public double FirstFreeMins { get; set; }
        #endregion

        #region 機車費用，當ProjType=4時才有值
        /// <summary>
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 基本消費
        /// </summary>
        public int BaseMinutesPrice { set; get; }
        /// <summary>
        /// 每分鐘多少-平日
        /// </summary>
        public float MinuteOfPrice { set; get; }
        /// <summary>
        /// 每分鐘多少-假日
        /// </summary>
        public float MinuteOfPriceH { get; set; }
        /// <summary>
        /// 每日上限金額
        /// </summary>
        public int MaxPrice { set; get; }
        #endregion

        #region 訂單相關
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string start_time { set; get; }
        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string final_start_time { set; get; }
        /// <summary>
        /// 取車截止時間
        /// </summary>
        public string stop_pick_time { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string stop_time { set; get; }
        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string final_stop_time { set; get; }
        /// <summary>
        /// 逾時開始時間
        /// </summary>
        public string fine_Time { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int init_price { set; get; }
        /// <summary>
        /// 是否有安心服務
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 預估安心保險費用
        /// </summary>
        public int InsurancePurePrice { set; get; }
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int init_TransDiscount { set; get; }
        /// <summary>
        /// 取還車狀態
        /// <para>0 = 尚未取車</para>
        /// <para>4 = 已經取車(記錄起始時間)</para>
        /// <para>11 = 已經紀錄還車時間</para>
        /// <para>16 = 已經檢查車輛完成並已經解除卡號</para>
        /// </summary>
        public int car_mgt_status { set; get; }
        /// <summary>
        /// 預約單狀態
        /// <para>0 = 會員預約</para>
        /// <para>1 = 管理員清潔預約</para>
        /// <para>3 = 延長用車狀態</para>
        /// <para>5 = 合約完成</para>
        /// </summary>
        public int booking_status { set; get; }
        /// <summary>
        /// 訂單修改狀態
        /// <para>0 = 無(訂單未刪除，正常預約狀態)</para>
        /// <para>3 = 訂單已取消</para>
        /// <para>5 = 整備使用(逾時未取或還車)</para>
        /// </summary>
        public int cancel_status { set; get; }
        /// <summary>
        /// 每公里n元
        /// </summary>
        public float MilageUnit { set; get; }
        /// <summary>
        /// 是否已經取車
        /// </summary>
        public int already_lend_car { set; get; }
        /// <summary>
        /// 前車是否已還車
        /// </summary>
        public int IsReturnCar { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 計算後的租金
        /// </summary>
        public int final_price { set; get; }
        /// <summary>
        /// 起始里程
        /// </summary>
        public float start_mile { set; get; }
        /// <summary>
        /// 結束里程
        /// </summary>
        public float end_mile { set; get; }
        /// <summary>
        /// 安心服務每小時價
        /// </summary>
        public float InsurancePerHours { set; get; }
        /// <summary>
        /// 平日價
        /// </summary>
        public int WeekdayPrice { get; set; }
        /// <summary>
        /// 假日售價
        /// </summary>
        public int HoildayPrice { get; set; }

        /// <summary>
        /// 副承租人每小時費率總和
        /// </summary>
        public int JointInsurancePerHour { get; set; }
        #endregion
    }

    /// <summary>
    /// 批次授權明細
    /// </summary>
    public class OrderAuthList
    {
        /// <summary>
        /// 批次授權序號
        /// </summary>
        public int authSeq { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int order_number { set; get; }
        /// <summary>
        /// 授權金額
        /// </summary>
        public int final_price { set; get; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// CardToken
        /// </summary>
        public string CardToken { set; get; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string transaction_no { set; get; }
    }


    /// <summary>
    /// 批次退款明細
    /// </summary>
    public class OrderAuthReturnList
    {
        /// <summary>
        /// 批次授權序號
        /// </summary>
        public int authSeq { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int order_number { set; get; }
        /// <summary>
        /// 退款金額
        /// </summary>
        public int returnAmt { set; get; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// CardToken
        /// </summary>
        public string CardToken { set; get; }
    }
}