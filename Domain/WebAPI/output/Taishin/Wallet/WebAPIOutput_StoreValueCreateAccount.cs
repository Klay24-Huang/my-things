using Domain.WebAPI.output.Taishin.Wallet.ResultParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet
{
    /// <summary>
    /// 直接儲值+開戶回傳
    /// </summary>
    public class WebAPIOutput_StoreValueCreateAccount
    {
        /// <summary>
        /// 輸出物件
        /// </summary>
        public StoreValueCreateAccountParam Result { get; set; }
        /// <summary>
        /// 回傳代碼，0000表成功
        /// </summary>
        public string ReturnCode { get; set; }
        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 回傳異常錯誤訊息 (僅在發生系統未預期錯誤時，此欄位方有值)
        /// </summary>
        public string ExceptionData { get; set; }
    }
}
