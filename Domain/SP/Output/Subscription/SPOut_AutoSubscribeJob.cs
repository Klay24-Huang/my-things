using System;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_AutoSubscribeJob
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// MonthlyRentId
        /// </summary>
        public int MonthlyRentId { get; set; }

        /// <summary>
        /// 方案代碼
        /// </summary>
        public string MonProjID { get; set; }

        /// <summary>
        /// 總期數
        /// </summary>
        public int MonProPeriod { get; set; }

        /// <summary>
        /// 短期總天數
        /// </summary>
        public int ShortDays { get; set; }

        /// <summary>
        /// 續訂檔ID
        /// </summary>
        public int SubsNxtID { get; set; }

        /// <summary>
        /// 是否為機車
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int IsMotor { get; set; }

        /// <summary>
        /// 下期方案ID
        /// </summary>
        public int NxtMonSetID { get; set; }

        /// <summary>
        /// 最後一期迄日
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 當期給付價格
        /// </summary>
        public int PeriodPayPrice { get; set; }

        /// <summary>
        /// 發票寄送方式
        /// <para>1:捐贈</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 手機條碼
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 發票寄送方式對應ID(TB_Code)
        /// </summary>
        public int InvoiceID { get; set; }
    }
}