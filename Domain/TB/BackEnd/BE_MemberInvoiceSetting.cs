using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int MEMIDNO { set; get; }
        /// <summary>
        /// 手機條碼/自然人憑證
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NOBAN { set; get; }
        /// <summary>
        /// 發票類別
        /// </summary>
        public string InvoiceType { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string UniCode { set; get; }
    }
}
