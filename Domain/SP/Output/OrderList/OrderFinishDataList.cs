using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.OrderList
{
    public class OrderFinishDataList
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { set; get; }
        /// <summary>
        /// 編號
        /// </summary>
        public int RowNo { set; get; }
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
        /// 統編
        /// </summary>
        public string UniCode { set; get; }

        /// <summary>
        /// 取車時間 月日時分
        /// </summary>
        public DateTime final_start_time { set; get; }
        /// <summary>
        /// 總租用時數
        /// </summary>
        public DateTime final_stop_time { set; get; }
        /// <summary>
        /// 總租金
        /// </summary>
        public int final_price { set; get; }

        /// <summary>
        /// 車輛圖顯示地區
        /// </summary>
        public string CarOfArea { set; get; }

        public string StationName { set; get; }

     
        /// <summary>
        /// 車輛圖片
        /// </summary>
        public string CarTypeImg { set; get; }
        /// <summary>
        /// 出租年
        /// </summary>
        public int RentYear { set; get; }
    }
}
