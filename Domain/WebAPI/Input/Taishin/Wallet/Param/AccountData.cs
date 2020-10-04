using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet.Param
{
    /// <summary>
    /// 轉贈者（轉入）資料
    /// </summary>
    public class AccountData
    {
        /// <summary>
        /// 商店傳入的轉入會員虛擬帳號
        /// </summary>
        public string TransferAccountId { get; set; }
        /// <summary>
        /// 轉入會員姓名
        /// </summary>
        public string TransferName { get; set; }
        /// <summary>
        /// 轉入手機號碼
        /// </summary>
        public string TransferPhoneNo { get; set; }
        /// <summary>
        /// 轉入Email
        /// </summary>
        public string TransferEmail { get; set; }
        /// <summary>
        /// 轉入證件號碼
        /// </summary>
        public string TransferID { get; set; }
    }
}
