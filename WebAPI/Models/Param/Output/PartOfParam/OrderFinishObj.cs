using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class OrderFinishObj
    {
        /// <summary>
        /// 年分
        /// </summary>
        public int RentYear { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 專案類型
        /// <para>0:同站</para>
        /// <para>3:路邊</para>
        /// <para>4:機車</para>
        /// </summary>
        public int ProjType { set; get; }
        /// <summary>
        /// 取車時間 月日時分
        /// </summary>
        public string RentDateTime { set; get; }
        /// <summary>
        /// 總租用時數
        /// </summary>
        public string TotalRentTime { set; get; }
        /// <summary>
        /// 總租金
        /// </summary>
        public int Bill { set; get; }
      
        /// <summary>
        /// 統編
        /// </summary>
        public string UniCode { set; get; }

        public string StationName { set; get; }

        /// <summary>
        /// 車輛圖顯示地區
        /// </summary>
        public string CarOfArea { set; get; }
        /// <summary>
        /// 車輛圖片
        /// </summary>
        public string CarTypePic { set; get; }

    }
}