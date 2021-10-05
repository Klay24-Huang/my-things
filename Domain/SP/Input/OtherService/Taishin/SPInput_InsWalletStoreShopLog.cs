using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsWalletStoreShopLog : SPInput_Base
    {

        /// <summary>
        /// 用戶端交易代碼
        /// </summary>
        public string CTxSn { get; set; }

        /// <summary>
        /// 交易序號
        /// </summary>
        public string TxSn { get; set; }

        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// 超商類型 (0:7-11 1:全家 2:萊爾富)
        /// </summary>
        public Int32 CvsType { get; set; }

        /// <summary>
        /// 超商代收碼
        /// </summary>
        public string CvsCode { get; set; }

        /// <summary>
        /// 繳費金額 
        /// </summary>
        public int PayAmount { get; set; }


        /// <summary>
        /// 期數 
        /// </summary>
        public Int32 PayPeriod { get; set; }

        /// <summary>
        /// 繳費期限 YYYYMMDD 
        /// </summary>
        public string DueDate { get; set; }


        /// <summary>
        /// 第一段條碼文字
        /// </summary>
        public string Code1 { get; set; }

        /// <summary>
        /// 第二段銷帳編號
        /// </summary>
        public string Code2 { get; set; }

        /// <summary>
        /// 第三段條碼文字
        /// </summary>
        public string Code3 { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 回應狀態代碼
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 狀態說明
        /// </summary>
        public string StatusDesc { get; set; }

        /// <summary>
        /// BarCode
        /// </summary>
        public string Barcode64 { get; set; }

        /// <summary>
        /// 短網址URL
        /// </summary>
        public string Url { get; set; }

    }
}
