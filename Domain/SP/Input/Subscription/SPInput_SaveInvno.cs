using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SaveInvno
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        /// <summary>
        /// 目前期數
        /// </summary>
        public int NowPeriod { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; }
        /// <summary>
        /// 發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string InvoiceType { set; get; }
        /// <summary>
        /// 載具條碼
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { set; get; }
        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string Invno { get; set; }
        /// <summary>
        /// 發票金額
        /// </summary>
        public int InvoicePrice { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string InvoiceDate { get; set; }
    }
}
