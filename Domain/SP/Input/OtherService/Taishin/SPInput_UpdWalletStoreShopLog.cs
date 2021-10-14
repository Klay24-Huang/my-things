using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_UpdWalletStoreShopLog : SPInput_Base
    {
  

        /// <summary>
        /// 交易類別 新增：i、刪除：d
        /// </summary>
        public string TxType { get; set; }

        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string PaymentId { get; set; }

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
        /// BarCode
        /// </summary>
        public string Barcode64 { get; set; }

        /// <summary>
        /// 短網址URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 回應狀態代碼
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 狀態說明
        /// </summary>
        public string StatusDesc { get; set; }

    }
}
