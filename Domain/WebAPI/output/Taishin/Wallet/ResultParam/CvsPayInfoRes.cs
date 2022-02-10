using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    /// <summary>
    /// 交易回覆物件
    /// </summary>
    public class CvsPayInfoRes
    {
        /// <summary>
        /// 繳費明細
        /// </summary>
        public List<CVSPayInfoDetailRes> detail { get; set; }

        /// <summary>
        /// 案件編號
        /// </summary>
        public string iHubTxId { get; set; }

        /// <summary>
        /// 筆數
        /// </summary>
        public Int32 recordCount { get; set; }

        /// <summary>
        /// 整批交易結果代碼
        /// </summary>
        public string statusCode { get; set; }

        /// <summary>
        /// 整批交易結果訊息
        /// </summary>
        public string statusDesc { get; set; }

        /// <summary>
        /// 用戶交易批號
        /// </summary>
        public string txBatchNo { get; set; }

        /// <summary>
        /// 交易類別 新增：i、刪除：d
        /// </summary>
        public string txType { get; set; }

    }

    public class CVSPayInfoDetailRes
    {
        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string paymentId { get; set; }

        /// <summary>
        /// iHub交易序號
        /// </summary>
        public string seqNo { get; set; }

        /// <summary>
        /// 交易明細結果代碼 S：成功、F：失敗
        /// </summary>
        public string statusCode { get; set; }

        /// <summary>
        /// 交易明細結果訊息
        /// </summary>
        public string statusDesc { get; set; }

    }

}
