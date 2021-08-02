using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_GetAccountStatus
    {
        /// <summary>
        /// 會員虛擬帳號（此欄位與會員虛擬帳號/台新訂單編號三擇一查詢）
        /// </summary>
        public string AccountId { get; set; }
    }
}