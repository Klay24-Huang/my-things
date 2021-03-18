using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class MonCardParam
    {
        /// <summary>
        /// 方案代碼-key
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 方案名稱
        /// </summary>
        public string MonProjNM { get; set; }
        /// <summary>
        /// 總期數-key
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 方案價格
        /// </summary>
        public int PeriodPrice { get; set; }
        /// <summary>
        /// 是否為機車0否1是
        /// </summary>
        public int IsMoto { get; set; }
        /// <summary>
        /// 汽車平日時數
        /// </summary>
        public int CarWDHours { get; set; }
        /// <summary>
        /// 汽車假日時數
        /// </summary>
        public int CarHDHours { get; set; }
        /// <summary>
        /// 汽車不分平假日時數
        /// </summary>
        public int CarTotalHours { get; set; }
        /// <summary>
        /// 機車平日時數
        /// </summary>
        public int MotoWDMins { get; set; }
        /// <summary>
        /// 機車假日時數
        /// </summary>
        public int MotoHDMins { get; set; }
        /// <summary>
        /// 機車不分平假日時數
        /// </summary>
        public int MotoTotalMins { get; set; }
        /// <summary>
        /// 方案起日-8碼
        /// </summary>
        public string SDATE { get; set; }
        /// <summary>
        /// 方案迄日-8碼
        /// </summary>
        public string EDATE { get; set; }
    }
}