using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletStoreTradeHistoryHidden
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 組織代號(公司代碼)
        /// </summary>
        public string ORGID { get; set; }
        /// <summary>
        /// 帳款流水號(by會員)
        /// </summary>
        public int SEQNO { get; set; }
        /// <summary>
        /// 財務-上游批號(IR編)
        /// </summary>
        public string F_INFNO { get; set; }
    }
}
