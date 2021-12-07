using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.CTBCPOS
{
    public class WebAPIOutput_CTBCBase
    {
        public int Status { get; set; }

        public string ErrCode { get; set; }

        /// <summary>
        /// 中信後台的交易識別碼
        /// </summary>
        public string XID { get; set; }

        /// <summary>
        /// 交易幣別代碼
        /// </summary>
        public string Currency { get; set; } = "901";

        /// <summary>
        /// (optional)為幣值指數，新台幣為 0。
        /// </summary>
        public int Exponent { get; set; } = 0;

        /// <summary>
        /// 授權交易之代碼
        /// </summary>
        public string AuthRRPID { get; set; }


        /// <summary>
        /// 交易授權碼
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; }

        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }

        /// <summary>
        /// 訊息規格版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 版本修訂日期
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorDesc { get; set; }

        /// <summary>
        /// 批次ID
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// 批次序號
        /// </summary>
        public int BatchSeq { get; set; }


        /// <summary>
        /// 此為保留欄位
        /// </summary>
        public int BatchClose { get; set; } = 0;
    }
}


