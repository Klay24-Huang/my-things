using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.MonthlyRent
{
    public class SPOut_GetMonthlyPayInv 
    {
        /// <summary>
        /// 方案代碼
        /// </summary>
        public string MonProjID { get; set; }

        /// <summary>
        /// 總期數
        /// </summary>
        public int MonProPeriod { get; set; }

        /// <summary>
        /// 短期總天數，非短期則為0
        /// </summary>
        public int ShortDays { get; set; }

        /// <summary>
        /// 目前期數
        /// </summary>
        public int NowPeriod { get; set; }

        /// <summary>
        /// 本期金額
        /// </summary>
        public int PreiodPrice { get; set; }

        /// <summary>
        /// 方案起日
        /// </summary>
        public string SDate { get; set; }

        /// <summary>
        /// 方案迄日
        /// </summary>
        public string EDate { get; set; }      

        /// <summary>
        /// 身分證
        /// </summary>
        public string IdNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string MemCName { get; set; }

        /// <summary>
        /// 電子郵件信箱
        /// </summary>
        public string MemEmail { get; set; }

        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 手機條碼
        /// </summary>
        public string CarrierId { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string Unimno { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string Npoban { get; set; }

        /// <summary>
        /// 是否為機車(1:是 2:否)
        /// </summary>
        public int IsMoto { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; }

        /// <summary>
        /// 發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; }

        /// <summary>
        /// 送出的交易序號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 台新交易序號(網刷編號)
        /// </summary>
        public string TaishinTradeNo { get; set; }

        /// <summary>
        /// 信用卡號
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; }

    }
}
