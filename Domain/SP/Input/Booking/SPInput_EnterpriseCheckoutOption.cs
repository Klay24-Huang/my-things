using System;

namespace Domain.SP.Input.Booking
{
    /// <summary>
    /// 從EASYRENT_WEB 查詢回來要寫入的值
    /// </summary>
    public class SPInput_EnterpriseCheckoutOption : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
        /// <summary>
        /// Etag是否為月結(0:否;1:是)
        /// </summary>
        public int Etag { set; get; }
        /// <summary>
        /// 安心服務是否為月結(0:否;1:是)
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 停車費是否為月結(0:否;1:是)
        /// </summary>
        public int Parking { set; get; }
    }
}