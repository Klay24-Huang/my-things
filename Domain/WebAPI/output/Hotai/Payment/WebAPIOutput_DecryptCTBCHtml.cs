using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class WebAPIOutput_DecryptCTBCHtml: WebAPOutput_PaymentBase
    {
        public string FullString { get; set; }

        public string Errcode { get; set; }
        /// <summary>
        /// 特店訂單編號(IRENT 編訂)
        /// </summary>
        public string Lidm { get; set; }
        /// <summary>
        /// 和泰OneID
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// 中信提供商代
        /// </summary>
        public string MerchantID { get; set; }
        /// <summary>
        /// 特店代碼(中信提供)
        /// </summary>
        public int MerID { get; set; }
        /// <summary>
        /// 詢問碼
        /// </summary>
        public string RequestNo { get; set; }
        /// <summary>
        /// 授權結果
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 授權結果代碼
        /// </summary>
        public string StatusCode { get; set; }
        /// <summary>
        /// 授權結果說明
        /// </summary>
        public string StatusDesc { get; set; }
        
        /*以下參數訂單成功才會有*/
        /// <summary>
        /// 授權金額
        /// </summary>
        public int AuthAmt { get; set; }
        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 授權交易之代碼
        /// </summary>
        public string Authrrpid { get; set; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 卡號末四碼
        /// </summary>
        public string Last4digitPAN { get; set; }
        /// <summary>
        /// 分期
        /// </summary>
        public int NumberOPay { get; set; }
        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; }
        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }
        /// <summary>
        /// 中信後台的交易識別碼
        /// </summary>
        public string Xid { get; set; }
        /// <summary>
        /// 請款批號
        /// </summary>
        public int CapBatchId { get; set; }
        /// <summary>
        /// 請款序號
        /// </summary>
        public int CapBatchSeq { get; set; }


    }
}
