using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.OrderList
{
    /// <summary>
    /// 取出取消的訂單列表
    /// </summary>
    public class OrderCancelDataList
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
        public Int64 order_number { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int init_price { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime start_time { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime stop_time { set; get; }
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
        /// 每公里多少錢
        /// </summary>
        public float MilageUnit { set; get; }
    }
}
