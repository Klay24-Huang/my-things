using Domain.SP.Input;
using System;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ContactFinish : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { get; set; }

        /// <summary>
        /// 強還時間
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// 發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
        /// </summary>
        public string bill_option { get; set; }

        /// <summary>
        /// 手機條碼載具,自然人憑證載具
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string unified_business_no { get; set; }

        /// <summary>
        /// 停車格
        /// </summary>
        public string ParkingSpace { get; set; }

        /// <summary>
        /// 動作用途
        /// <para>0:會員</para>
        /// <para>1:清潔取還車</para>
        /// <para>2:保修取還車</para>
        /// <para>3:系統操作異常</para>
        /// <para>4:逾時未還</para>
        /// <para>5:營運範圍外無法還車</para>
        /// <para>6:車輛沒電</para>
        /// <para>7:其他</para>
        /// </summary>
        public Int16 Mode { get; set; }
    }
}