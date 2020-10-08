using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_OrderDetail
    {
        /// <summary>
        /// 合約網址
        /// </summary>
        public string ContactURL { set; get; }
              /// <summary>
              /// 總租用時數
              /// </summary>
        public string TotalRentTime { set; get; }
        /// <summary>
        /// 行駛總里程
        /// </summary>
        public double TotalMile { set; get; }
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
        /// 開始時間
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { set; get; }
    }
}