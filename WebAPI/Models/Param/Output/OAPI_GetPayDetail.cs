using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 租金明細輸出
    /// </summary>
    public class OAPI_GetPayDetail
    {
        /// <summary>
        /// 是否可使用點數折扣
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int CanUseDiscount { set; get; }
        /// <summary>
        /// 是否可使用月租時間
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int CanUseMonthRent { set; get; }
        /// <summary>
        /// 是否為月租
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMonthRent { set; get; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
        /// <summary>
        /// 使用訂金
        /// </summary>
        public int UseOrderPrice { get; set; }
        /// <summary>
        /// 返還訂金
        /// </summary>
        public int ReturnOrderPrice { get; set; }
        /// <summary>
        /// 沒收訂金
        /// </summary>
        public int FineOrderPrice { get; set; }
        /// <summary>
        /// 訂單基本資訊
        /// </summary>
        public RentBase Rent { set; get; }
        /// <summary>
        /// 汽車相關資料
        /// </summary>
        public CarRentBase CarRent { set; get; }
        /// <summary>
        /// 機車的相關資訊
        /// </summary>
        public MotorRentBase MotorRent { set; get; }
        /// <summary>
        /// 月租相關資訊
        /// </summary>
        public MonthRentBase MonthRent { set; get; }
        /// <summary>
        /// 月租下拉
        /// </summary>
        public List<MonBase> MonBase { get; set; }
        /// <summary>
        /// 專案類型
        /// <para>0:同站</para>
        /// <para>3:路邊</para>
        /// <para>4:機車</para>
        /// </summary>
        public int ProType { set; get; }
        /// <summary>
        /// 計費模式
        /// <para>0:以時計費</para>
        /// <para>1:以分計費</para>
        /// </summary>
        public int PayMode { set; get; }

        /// <summary>
        /// 不可使用折抵時的訊息提示
        /// </summary>
        public string DiscountAlertMsg { set; get; }
 
        /// <summary>
        /// 目前可使用訂閱制月租
        /// </summary>
        public List<OAPI_NowSubsCard> NowSubsCards { get; set; }
    }
}