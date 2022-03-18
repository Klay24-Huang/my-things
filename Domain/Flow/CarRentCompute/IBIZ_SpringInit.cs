using Domain.TB;
using System;
using System.Collections.Generic;

namespace Domain.Flow.CarRentCompute
{
    public class IBIZ_SpringInit
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; } = 0;

        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }

        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { set; get; }

        /// <summary>
        /// 取車時間
        /// </summary>
        public DateTime SD { set; get; }

        /// <summary>
        /// 還車時間
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 平日價-小時
        /// </summary>
        public double PRICE { set; get; }

        /// <summary>
        /// 假日價-小時
        /// </summary>
        public double PRICE_H { set; get; }

        /// <summary>
        /// 專案平日價-小時
        /// </summary>
        public double ProDisPRICE { set; get; }

        /// <summary>
        /// 專案假日價-小時
        /// </summary>
        public double ProDisPRICE_H { set; get; }

        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; } = new List<Holiday>();

        /// <summary>
        /// LogID
        /// </summary>
        public long LogID { set; get; }
    }
}