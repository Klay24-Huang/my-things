using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 進行中的訂單（含未取車）
    /// </summary>
    public class ActiveOrderData
    {
        #region 據點相關
        ///// <summary>
        ///// 據點名稱，對應tb裡的Location
        ///// </summary>
        //public string StationName { set; get; }
        ///// <summary>
        ///// 地址
        ///// </summary>
        //public string ADDR { set; get; }
        ///// <summary>
        ///// 緯度
        ///// </summary>
        //public float Latitude { set; get; }
        ///// <summary>
        ///// 經度
        ///// </summary>
        //public float Longitude { set; get; }
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
        /// 取車截止時間
        /// </summary>
        public string StopPickTime { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public string StopTime { set; get; }
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
        #endregion
    }
}