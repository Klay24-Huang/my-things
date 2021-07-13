using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_CreateSubsMonth
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; }
        /// <summary>
        /// 發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; }
        public DateTime? SetNow { get; set; } = null;//不用填
        public Int64 SetSubGrop { get; set; } = 0;//不用填
        /// <summary>
        /// 首期付款寫入-非必填
        /// </summary>
        public int SetPayOne { get; set; } = 0;
        /// <summary>
        /// 設定自動續約-非必填
        /// </summary>
        public int SetSubsNxt { get; set; } = 0;
        /// <summary>
        /// 台新送出的訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 台新的交易序號
        /// </summary>
        public string TaishinTradeNo { get; set; }
    }
}
