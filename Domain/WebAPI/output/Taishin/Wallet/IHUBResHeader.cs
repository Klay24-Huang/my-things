using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet
{
    /// <summary>
    /// 交易回覆共通物件
    /// </summary>
    public class IHUBResHeader
    {
        /// <summary>
        /// 用戶Id
        /// </summary>
        public string cId { get; set; }

        /// <summary>
        ///客戶端自訂交易序號
        /// </summary>
        public string cTxSn { get; set; }

        /// <summary>
        /// 外部資源代碼
        /// </summary>
        public string extRes { get; set; }

        /// <summary>
        /// 外部資源回覆代碼
        /// </summary>
        public string extRtnCode { get; set; }

        /// <summary>
        /// 外部資源回覆訊息
        /// </summary>
        public string extRtnMsg { get; set; }

        /// <summary>
        /// 回覆代碼
        /// </summary>
        public string rtnCode { get; set; }

        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string rtnMsg { get; set; }

        /// <summary>
        /// 回覆子代碼
        /// </summary>
        public string subCode { get; set; }
        /// <summary>
        /// 交易時間(同Reqeust)
        /// </summary>
        public string txDate { get; set; }

        /// <summary>
        /// 交易序號(台新回傳)
        /// </summary>
        public string txSN { get; set; }

    }
}
