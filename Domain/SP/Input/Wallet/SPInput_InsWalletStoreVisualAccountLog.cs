using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsWalletStoreVisualAccountLog : SPInput_Base
    {
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 虛擬帳號
        /// </summary>
        public string TrnActNo { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        public string Amt { get; set; }

        /// <summary>
        /// 繳費期限
        /// </summary>
        public DateTime DueDate { get; set; }

    }
}
