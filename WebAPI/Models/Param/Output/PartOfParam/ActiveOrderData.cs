using Domain.TB;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 進行中的訂單（含未取車）
    /// </summary>
    public class ActiveOrderData
    {
        #region 據點相關
        /// <summary>
        /// 據點資訊
        /// </summary>
        public iRentStationData StationInfo { set; get; }
        #endregion
        #region 車輛相關資料
        /// <summary>
        /// 營運商
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 評分
        /// </summary>
        public float OperatorScore { set; get; }
        /// <summary>
        /// 車輛圖片
        /// </summary>
        public string CarTypePic { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 廠牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 座椅數
        /// </summary>
        public int Seat { set; get; }
        /// <summary>
        /// 停車格位置
        /// </summary>
        public string ParkingSection { set; get; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
        /// <summary>
        /// 車輛圖顯示地區
        /// </summary>
        public string CarOfArea { set; get; }

        /// <summary>
        /// 車緯度
        /// </summary>
        public decimal CarLatitude { get; set; }

        /// <summary>
        /// 車經度
        /// </summary>
        public decimal CarLongitude { get; set; }

        /// <summary>
        /// 機車電力資訊，當ProjType=4時才有值
        /// </summary>
        public MotorPowerInfoBase MotorPowerBaseObj { set; get; }
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
        public string ProjName { set; get; }
        /// <summary>
        /// 平日每小時
        /// </summary>
        public int WorkdayPerHour { set; get; }
        /// <summary>
        /// 假日每小時
        /// </summary>
        public int HolidayPerHour { set; get; }
        /// <summary>
        /// 每日上限
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 假日上限
        /// </summary>
        public int MaxPriceH { set; get; }
        /// <summary>
        /// 機車費用，當ProjType=4時才有值
        /// </summary>
        public MotorBillBase MotorBasePriceObj { set; get; }
        #endregion
        #region 訂單相關(汽車)
        /// <summary>
        /// 訂單狀態
        /// -1:前車未還（未到站）
        ///  0:可取車
        ///  1:用車中
        ///  2:延長用車中
        ///  3:準備還車
        ///  4:逾時
        ///  5:還車流程中（未完成還車）
        /// </summary>
        public int OrderStatus { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string PickTime { set; get; }
        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string ReturnTime { get; set; }
        /// <summary>
        /// 取車截止時間
        /// </summary>
        public string StopPickTime { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string StopTime { set; get; }
        /// <summary>
        /// 使用期限
        /// </summary>
        public string OpenDoorDeadLine { get; set; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int CarRentBill { set; get; }
        /// <summary>
        /// 每一公里里程費
        /// </summary>
        public float MileagePerKM { set; get; }
        /// <summary>
        /// 預估里程費
        /// </summary>
        public int MileageBill { set; get; }

        /// <summary>
        /// 是否可以使用安心服務
        /// </summary>
        public int Insurance { get; set; }

        /// <summary>
        /// 安心保險每小時
        /// </summary>
        public int InsurancePerHour { set; get; }
        /// <summary>
        /// 預估安心保險費用
        /// </summary>
        public int InsuranceBill { set; get; }
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; }
        /// <summary>
        /// 預估總金額
        /// </summary>
        public int Bill { set; get; }
        /// <summary>
        /// 單日計費上限時數
        /// </summary>
        public int DailyMaxHour { get; set; }
        /// <summary>
        /// 取還車狀態
        /// 0 = 尚未取車
        /// 1 = 已經上傳出車照片
        /// 2 = 已經簽名出車單
        /// 3 = 已經信用卡認證
        /// 4 = 已經取車(記錄起始時間)
        /// 11 = 已經紀錄還車時間
        /// 12 = 已經上傳還車角度照片
        /// 13 = 已經上傳還車車損照片
        /// 14 = 已經簽名還車單
        /// 15 = 已經信用卡付款
        /// 16 = 已經檢查車輛完成並已經解除卡號
        /// </summary>
        public int CAR_MGT_STATUS { get; set; }
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
        public int AppStatus { get; set; }

        /// <summary>
        /// 承租人類型
        /// <para>1:主要承租人</para>
        /// <para>2:共同承租人</para>
        /// </summary>
        public int RenterType { get; set; }
        /// <summary>
        /// 前車圖片路徑
        /// </summary>
        public string PreviousCarPath { set; get; }
        #endregion
        #region 優惠相關
        /// <summary>
        /// 優惠標籤
        /// </summary>
        public DiscountLabel DiscountLabel { get; set; }
        #endregion
        /// <summary>
        /// 機車安心服務低消分鐘數
        /// </summary>
        public float BaseInsuranceMinutes { get; set; }
        /// <summary>
        /// 機車安心服務低消金額
        /// </summary>
        public float BaseMotoRate { get; set; }
        /// <summary>
        /// 機車安心服務分鐘(幾分鐘算一次錢)
        /// </summary>
        public float InsuranceMotoMin { get; set; }
        /// <summary>
        /// 機車安心服務金額(一次算多少錢)
        /// </summary>
        public float InsuranceMotoRate { get; set; }

        /// <summary>
        /// 是否為企業客戶訂單 1:是 0:否
        /// </summary>
        public int IsEnterpriseOrder { get; set; }
    }

}