﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 路邊租還輸出
    /// </summary>
    public class AnyRentObj
    {
        public string CarNo { set; get; }
        public string CarTypeName { set; get; }
        public string CarOfArea { set; get; }
        public string ProjectName { set; get; }
        public float Rental { set; get; }
        public float Mileage { set; get; }
        /// <summary>
        /// 是否有安心服務
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 安心服務每小時費用
        /// </summary>
        public int InsurancePrice { set; get; }
        /// <summary>
        /// 是否顯示活動文字
        /// </summary>
        public int ShowSpecial { set; get; }
        /// <summary>
        /// 活動文字
        /// </summary>
        public string SpecialInfo { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 供應商
        /// </summary>
        public string OperatorICon { get; set; }
        /// <summary>
        /// 幾顆星評價
        /// </summary>
        public float Score { set; get; }
        /// <summary>
        /// 車輛圖示
        /// </summary>
        public string CarTypeImg { get; set; }
        /// <summary>
        /// 座位數
        /// </summary>
        public int Seat { get; set; }
    }
}
