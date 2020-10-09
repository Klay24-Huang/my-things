using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Mochi
{
    public class WebAPIOutput_QueryBillByCar
    {
        public PagingObj paging { set; get; }
        public DataObj[] data { set; get; }
    }
    public class PagingObj
    {
        /// <summary>
        /// 總頁數
        /// </summary>
        public int total_page { set; get; }
        /// <summary>
        /// 每頁幾筆
        /// </summary>
        public int per_page { set; get; }
        /// <summary>
        /// 目前頁碼
        /// </summary>
        public int current_page { set; get; }
    }
    public class DataObj
    {
        /// <summary>
        /// 車麻吉訂單編號
        /// </summary>
        public string id { set; get; }
        public DistributorObj distributor { set; get; }
        /// <summary>
        /// 停車費訂單類型
        /// </summary>
        public string type { set; get; }
        /// <summary>
        /// 交易類型
        /// <para>parking:停車場</para>
        /// </summary>
        public string transaction_type { set; get; }
        /// <summary>
        /// 停車場id
        /// </summary>
        public string store_id { set; get; }
        /// <summary>
        /// 停車場類型
        /// </summary>
        public string store_type { set; get; }
        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string store_name { set; get; }
        public string amount { get; set; }
        public string payment_state { get; set; }
        public string plate_number { get; set; }
        public DateTime paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string refund_state { get; set; }
        public string refunded_amount { get; set; }
        public DetailObj details { get; set; }

    }
    public class DistributorObj
    {
        /// <summary>
        /// 車麻吉名稱
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 車麻吉帳號
        /// </summary>
        public string uid { set; get; }
    }
    public partial class DetailObj
    {
        /// <summary>
        /// 入場時間
        /// </summary>
        public DateTime parking_checked_in_at { get; set; }
        /// <summary>
        /// 出場時間
        /// </summary>
        public DateTime parking_checked_out_at { get; set; }
    }
}
