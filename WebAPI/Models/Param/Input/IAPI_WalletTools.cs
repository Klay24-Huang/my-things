using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTools
    {
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 功能名稱 目前支援以下項目
        /// AccountStatus
        /// AccountValue
        /// PayTransaction 儲值退款
        /// </summary>
        public string FuncName { get; set; }
        /// <summary>
        /// 交易金額 
        /// </summary>
        public int PayAmount { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string USERID { get; set; }
    }
}