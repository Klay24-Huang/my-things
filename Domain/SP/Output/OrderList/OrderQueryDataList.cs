using System;

namespace Domain.SP.Output.OrderList
{
    public class OrderQueryDataList
    {
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

        public string StationPic1 { set; get; }
        public string StationPic2 { set; get; }
        public string StationPic3 { set; get; }
        public string StationPic4 { set; get; }
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
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
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
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }

        /// <summary>
        /// 車緯度
        /// </summary>
        public decimal CarLatitude { get; set; }

        /// <summary>
        /// 車經度
        /// </summary>
        public decimal CarLongitude { get; set; }
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

        #region 機車費用，當ProjType=4時才有值
        /// <summary>
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 低消
        /// </summary>
        public int BaseMinutesPrice { set; get; }
        /// <summary>
        /// 每分鐘多少
        /// </summary>
        public float MinuteOfPrice { set; get; }
        /// <summary>
        /// 每日上限
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 假日上限
        /// </summary>
        public int MaxPriceH { set; get; }
        #endregion
        #endregion
        #region 訂單相關
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 order_number { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string start_time { set; get; }
        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string final_start_time { set; get; }
        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string final_stop_time { get; set; }
        /// <summary>
        /// 取車截止時間
        /// </summary>
        public string stop_pick_time { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string stop_time { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int init_price { set; get; }
        /// <summary>
        /// 預估每小時安心保險費用
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
        /// >4=>已取車
        /// >=11=>已點還車
        /// 15=>已過還車金流
        /// 16=>完成還車
        /// </summary>
        public int car_mgt_status { set; get; }
        /// <summary>
        /// 5=>完成還車
        /// 3=>延長用車
        /// </summary>
        public int booking_status { set; get; }
        /// <summary>
        /// 大於0取車
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
        /// <summary> 20201026 ADD BY ADAM
        /// 1:尚未到取車時間(取車時間半小時前)
        /// 2:立即換車(取車前半小時，前車尚未完成還車)
        /// 3:開始使用(取車時間半小時前)
        /// 4:開始使用-提示最晚取車時間(取車時間後~最晚取車時間)
        /// 5:操作車輛(取車後) 取車時間改實際取車時間
        /// 6:操作車輛(準備還車)-
        /// 7:物品遺漏(再開一次車門)
        /// 8:鎖門並還車(一次性開門申請後)
        /// </summary>
        public int AppStatus { set; get; }
        #endregion
    }
}