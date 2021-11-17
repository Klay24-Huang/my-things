using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Bill
{
    public class HotaiCardInfo: CreditCardInfo
    {
    
        /// <summary>
        /// 和泰會員OneID
        /// </summary>
        public string MemberOneID { get; set; }

    }
}