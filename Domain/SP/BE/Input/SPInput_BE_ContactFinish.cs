using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ContactFinish:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        public string UserID { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// 強還時間
        /// </summary>
        public DateTime ReturnDate { set; get; }
        /// <summary>
        /// 發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
        /// </summary>
        public string bill_option { set; get; }
        /// <summary>
        /// 手機條碼載具,自然人憑證載具
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string unified_business_no { set; get; }
        /// <summary>
        /// 停車格
        /// </summary>
        public string ParkingSpace { set; get; }
    }
}
