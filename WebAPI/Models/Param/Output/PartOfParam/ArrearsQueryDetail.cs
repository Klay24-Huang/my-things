using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 欠費明細
    /// </summary>
    public class ArrearsQueryDetail
    {
        ///// <summary>
        ///// 欠費種類 1:租金,2:罰單,3:停車費,4:ETAG
        ///// </summary>
        //public string ArrearsKind { set; get; }
        ///// <summary>
        ///// iRent合約編號
        ///// </summary>
        //public string OrderNo { set; get; }
        ///// <summary>
        ///// 短租合約編號
        ///// </summary>
        //public string ShortOrderNo { set; get; }
        ///// <summary>
        ///// 取車據點
        ///// </summary>
        //public string StationName { set; get; }
        ///// <summary>
        ///// 實際取車時間
        ///// </summary>
        //public string StartDate { set; get; }
        ///// <summary>
        ///// 實際還車時間
        ///// </summary>
        //public string EndDate { set; get; }
        ///// <summary>
        ///// 車型代碼，需左補0到六碼
        ///// </summary>
        //public string CarType { set; get; }
        ///// <summary>
        ///// 是否是機車
        ///// <para>0:否</para>
        ///// <para>1:是</para>
        ///// </summary>
        //public int IsMotor { set; get; } 
        ///// <summary>
        ///// 待繳金額
        ///// </summary>
        //public int Amount { set; get; }

        public int NPR330Save_ID { get; set; }
        /// <summary>
        /// 租金
        /// </summary>
        public int Rent_Amount { get; set; }
        /// <summary>
        /// 罰單
        /// </summary>
        public int Ticket_Amount { get; set; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int Park_Amount { get; set; }
        /// <summary>
        /// ETAG
        /// </summary>
        public int ETAG_Amount { get; set; }
        /// <summary>
        /// 營損,代收停車費	
        /// </summary>
        public int OperatingLoss_Amount { get; set; }
        /// <summary>
        /// 欠費總和
        /// </summary>
        public int Total_Amount { get; set; }
        /// <summary>
        /// 實際取車時間
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 實際還車時間
        /// </summary>
        public string EndDate { set; get; }
        /// <summary>
        /// iRent合約編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 短租合約編號
        /// </summary>
        public string ShortOrderNo { set; get; }
        /// <summary>
        /// 取車據點
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 車型代碼，需左補0到六碼
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 是否是機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
    }
}