using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_ChangeCreditCardBind
    {
        /// <summary>
        /// 信用卡/銀行帳戶虛擬代碼
        /// </summary>
        public string CardToken { get; set; }
    }
}