using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_CarRentInCompute
    {
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        /// <summary>
        /// 專案平日價
        /// </summary>
        public double priceN { get; set; }
        /// <summary>
        /// 專案假日價
        /// </summary>
        public double priceH { get; set; }
        /// <summary>
        /// 基本分鐘60
        /// </summary>
        public double daybaseMins { get; set; }
        /// <summary>
        /// 計費單日最大小時數10
        /// </summary>
        public double dayMaxHour { get; set; }
        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; }
        /// <summary>
        /// 月租列表
        /// </summary>
        public List<MonthlyRentData> mOri { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 前n免費
        /// </summary>
        public double FreeMins { get; set; }
    }

}