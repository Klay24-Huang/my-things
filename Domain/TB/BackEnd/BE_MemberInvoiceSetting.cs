namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 縣市資料
    /// </summary>
    public  class BE_MemberInvoiceSetting
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 手機條碼/自然人憑證
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 發票類別
        /// </summary>
        public string InvoiceType { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string UniCode { set; get; }
        /// <summary>
        /// 停車格
        /// </summary>
        public string ParkingSpace { set; get; }

        /// <summary>
        /// 出車據點
        /// </summary>
        public string lend_place { get; set; }

        /// <summary>
        /// CID
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        /// 車輛是否在據點範圍內還車 (Y:範圍內 N:範圍外)
        /// </summary>
        public string IsArea { get; set; } = "Y";
    }
}