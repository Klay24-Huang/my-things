using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Maintain
{
    public class CarCleanDataNew
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 所屬據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 目前據點代碼
        /// </summary>
        public string NowStationID { set; get; }
        /// <summary>
        /// 目前據點名稱
        /// </summary>
        public string NowStationName { set; get; }
        /// <summary>
        /// 上線狀態
        /// <para>0:出租中</para>
        /// <para>1:可出租</para>
        /// <para>2:下線</para>
        /// </summary>
        public int online { set; get; }
        /// <summary>
        /// 是否為租約狀態
        /// <para>0:非出租狀態</para>
        /// <para>大於0，為出租狀態，數字為訂單編號</para>
        /// </summary>
        public int OrderStatus { set; get; }
        /// <summary>
        /// 預計開始
        /// </summary>
        public DateTime BookingStart { set; get; }
        /// <summary>
        /// 預計結束
        /// </summary>
        public DateTime BookingEnd { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Lat { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Lng { set; get; }
        /// <summary>
        /// 回傳的GPS時間
        /// </summary>
        public DateTime GPSTime { set; get; }
        /// <summary>
        /// 最近一次清潔時間
        /// </summary>
        public DateTime LastClean { set; get; }
        /// <summary>
        /// 清潔後出租次數
        /// </summary>
        public int AfterRent { set; get; }
        /// <summary>
        /// 管轄據點
        /// </summary>
        public string ManageStationID { set; get; }
        /// <summary>
        /// 出租次數
        /// </summary>
        public int NewRentCount { set; get; }
        /// <summary>
        /// 未清潔次數
        /// </summary>
        public int NewCleanCount { set; get; }
        /// <summary>
        /// 上次清潔結束時間
        /// </summary>
        public DateTime LastCleanTime { set; get; }
        /// <summary>
        /// 上次出租還車時間
        /// </summary>
        public DateTime LastRentTime { set; get; }
        /// <summary>
        /// 上次保養里程
        /// </summary>
        public Int64 LastMaintenanceMilage { set; get; }
        public int LowPowStatus { set; get; }
        public Int64 Milage { set; get; }
        /// <summary>
        /// 間距前一次出租天數
        /// </summary>
        public int afterRentDays { set; get; }
        /// <summary>
        /// 一小時未回應
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int isNoResponse { set; get; }
        /// <summary>
        /// 是否需要保養
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int isNeedMaintenance { set; get; }
        /// <summary>
        /// 是否保修或清潔中
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int OnMaintain { get; set; }
    }
}
