using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 取消訂單資訊
    /// </summary>
    public class OrderCancelObj
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string order_number { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int Price { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 座椅數
        /// </summary>
        public int Seat { set; get; }
        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 分數
        /// </summary>
        public float Score { set; get; }
        public string OperatorICon { set; get; }
        /// <summary>
        /// 車型圖
        /// </summary>
        public string CarTypeImg { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string PRONAME { set; get; }
        /// <summary>
        /// 預估每小時多少公里
        /// </summary>
        public int MilOfHours { set; get; }
        /// <summary>
        /// 每公里多少朋
        /// </summary>
        public float MilageUnit { set; get; }
        /// <summary>
        /// 預估里程費
        /// </summary>
        public int Milage { set; get; }
        /// <summary>
        /// 站別類型
        /// </summary>
        public string CarOfArea { get; set; }
        /// <summary>
        /// 站別名稱
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
        /// <summary>
        /// 平日價
        /// </summary>
        public float WeekdayPrice { get; set; }
        /// <summary>
        /// 假日售價
        /// </summary>
        public float HoildayPrice { get; set; }
        public float WeekdayPriceByMinutes { get; set; }
        public float HoildayPriceByMinutes { get; set; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int CarRentBill { set; get; }
        /// <summary>
        /// 預估安心保險費用
        /// </summary>
        public int InsuranceBill { set; get; }
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; }
        /// <summary>
        /// 預估里程費
        /// </summary>
        public int MileageBill { set; get; }
        /// <summary>
        /// 預估總金額
        /// </summary>
        public int Bill { set; get; }
    }
}