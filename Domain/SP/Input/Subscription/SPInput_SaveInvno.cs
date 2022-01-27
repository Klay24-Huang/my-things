﻿using System;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_SaveInvno : SPInput_Base
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// MonthlyRentID
        /// </summary>
        public long MonthlyRentID { get; set; } = 0;
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
        /// <summary>
        /// 程式來源
        /// </summary>
        public string PRGID { get; set; } = "";
    }
}