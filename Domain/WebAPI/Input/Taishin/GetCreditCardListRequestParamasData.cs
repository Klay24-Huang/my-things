using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    /// <summary>
    /// 台新_取回綁定(信用卡|銀行帳號)列表
    /// </summary>
   public  class GetCreditCardListRequestParamasData
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string MemberId { set; get; }
    }
}
