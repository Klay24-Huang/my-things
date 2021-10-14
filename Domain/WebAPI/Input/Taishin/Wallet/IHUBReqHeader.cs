using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 交易請求共通物件
    /// </summary>
    public class IHUBReqHeader
    {
        /// <summary>
        /// 客戶端自訂交易序號
        /// </summary>
        public string cTxSn { get; set; }

        /// <summary>
        /// 含時區資訊的時戳, ex .20180812T173301+0800
        /// </summary>
        public string txDate { get; set; }

        /// <summary>
        /// 交易代碼,ex .IHUB000101
        /// </summary>
        public string txId { get; set; }

        /// <summary>
        /// 用戶Id
        /// </summary>
        public string cId { get; set; }
    }
}
