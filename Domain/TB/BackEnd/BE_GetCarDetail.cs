using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 車輛明細資料
    /// </summary>
    public class BE_GetCarDetail
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 出租次數
        /// </summary>
        public int RentCount { set; get; }
        /// <summary>
        /// 未清潔次數
        /// </summary>
        public int UncleanCount { set; get; }
        /// <summary>
        /// 假日價-汽車
        /// </summary>
        public int HoildayPrice { set; get; }
        /// <summary>
        /// 平日價-汽車
        /// </summary>
        public int WeekdayPrice { set; get; }
        /// <summary>
        /// 座位數
        /// </summary>
        public Int16 Seat { set; get; }
        /// <summary>
        /// 出廠年月
        /// </summary>
        public string FactoryYear { set; get; }
        /// <summary>
        /// 車色
        /// </summary>
        public string CarColor { set; get; }
        /// <summary>
        /// 引擎編號
        /// </summary>
        public string EngineNO { set; get; }
        /// <summary>
        /// 車身編號
        /// </summary>
        public string BodyNO { set; get; }
        /// <summary>
        /// cc數
        /// </summary>
        public int CCNum { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 是否是興聯車機
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsCens { set; get; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsMotor { set; get; }
        /// <summary>
        /// 所屬據點
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 目前據點
        /// </summary>
        public string NowStationName { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 目前狀態
        /// <para>0:出租中</para>
        /// <para>1:可出租</para>
        /// <para>2:待上線</para>
        /// </summary>
        public Int16 NowStatus { set; get; }
        /// <summary>
        /// 機車假日每分鐘
        /// </summary>
        public float HoildayPriceByMinutes { set; get; }
        /// <summary>
        /// 機車平日每分鐘
        /// </summary>
        public float WeekdayPriceByMinutes { set; get; }
        /// <summary>
        /// 是否有安裝iButton
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 HasIButton { set; get; }
        /// <summary>
        /// iButtonKey
        /// </summary>
        public string iButtonKey { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { set; get; }
        /// <summary>
        /// 門號
        /// </summary>
        public string MobileNo { set; get; }
    }
}
