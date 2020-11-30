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
   public  class BE_MemberInvoiceSettingData
    {
        /// <summary>
        /// 會員資料
        /// </summary>
        public List<BE_MemberInvoiceSetting> MemberInvoice { set; get; }
        /// <summary>
        /// 愛心碼清單
        /// </summary>
        public List<LoveCodeListData> LoveCodeList { set; get; }
    }
}
