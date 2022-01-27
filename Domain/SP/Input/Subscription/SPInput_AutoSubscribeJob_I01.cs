namespace Domain.SP.Input.Subscription
{
    public class SPInput_AutoSubscribeJob_I01 : SPInput_Base
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
        /// 付款方式
        /// </summary>
        public int PayTypeId { get; set; }

        /// <summary>
        /// 發票設定
        /// </summary>
        public int InvoTypeId { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 交易序號
        /// </summary>
        public string TaishinTradeNo { get; set; }

        /// <summary>
        /// 來源程式
        /// </summary>
        public string PRGID { get; set; }
    }
}